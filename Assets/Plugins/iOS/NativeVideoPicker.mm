#import <UIKit/UIKit.h>
#import <PhotosUI/PhotosUI.h>
#import <Photos/Photos.h>
#import <AVFoundation/AVFoundation.h>
#import <MobileCoreServices/MobileCoreServices.h>
#import "UnityInterface.h"

// Global variables to store Unity callback objects
extern "C" {
    void UnitySendMessage(const char* obj, const char* method, const char* msg);
}

@interface VideoPickerDelegate : NSObject <PHPickerViewControllerDelegate, UIImagePickerControllerDelegate, UINavigationControllerDelegate>
@property (nonatomic, strong) UIViewController *pickerController;
- (int)getVideoOrientation:(NSURL *)videoURL;
@end

@implementation VideoPickerDelegate

- (void)picker:(PHPickerViewController *)picker didFinishPicking:(NSArray<PHPickerResult *> *)results API_AVAILABLE(ios(14.0)) {
    [picker dismissViewControllerAnimated:YES completion:nil];

    if (results.count == 0) {
        // User cancelled
        UnitySendMessage("VideoPickerManager", "OnVideoPickerCancelled", "");
        return;
    }

    PHPickerResult *result = results.firstObject;

    if ([result.itemProvider hasItemConformingToTypeIdentifier:(NSString *)kUTTypeMovie]) {
        [result.itemProvider loadFileRepresentationForTypeIdentifier:(NSString *)kUTTypeMovie completionHandler:^(NSURL * _Nullable url, NSError * _Nullable error) {
            if (url && !error) {
                // Copy the video to app's documents directory for reliable access
                NSFileManager *fileManager = [NSFileManager defaultManager];
                NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
                NSString *documentsDirectory = [paths objectAtIndex:0];
                NSString *fileName = [[url lastPathComponent] stringByDeletingPathExtension];
                NSString *extension = [url pathExtension];
                NSString *destinationPath = [documentsDirectory stringByAppendingPathComponent:[NSString stringWithFormat:@"%@.%@", fileName, extension]];

                NSError *copyError = nil;
                // If file exists, remove it first
                if ([fileManager fileExistsAtPath:destinationPath]) {
                    [fileManager removeItemAtPath:destinationPath error:nil];
                }
                if ([fileManager copyItemAtURL:url toURL:[NSURL fileURLWithPath:destinationPath] error:&copyError]) {
                    // Read video orientation from metadata
                    int orientation = [self getVideoOrientation:url];
                    NSString *message = [NSString stringWithFormat:@"%@|%d", destinationPath, orientation];
                    NSLog(@"Video copied to: %@ with orientation: %d", destinationPath, orientation);
                    UnitySendMessage("VideoPickerManager", "OnVideoSelected", [message UTF8String]);
                } else {
                    NSLog(@"Error copying video: %@", copyError);
                    UnitySendMessage("VideoPickerManager", "OnVideoPickerCancelled", "");
                }
            } else {
                NSLog(@"Error loading video: %@", error);
                UnitySendMessage("VideoPickerManager", "OnVideoPickerCancelled", "");
            }
        }];
    } else {
        // Not a video
        UnitySendMessage("VideoPickerManager", "OnVideoPickerCancelled", "");
    }
}

- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary<NSString *,id> *)info {
    NSString *mediaType = [info objectForKey:UIImagePickerControllerMediaType];

    if ([mediaType isEqualToString:(NSString *)kUTTypeMovie]) {
        NSURL *videoURL = [info objectForKey:UIImagePickerControllerMediaURL];

        if (videoURL) {
            NSString *videoPath = [videoURL path];
            NSLog(@"Video selected: %@", videoPath);

            // Send message to Unity
            UnitySendMessage("VideoPickerManager", "OnVideoSelected", [videoPath UTF8String]);
        }
    }

    [picker dismissViewControllerAnimated:YES completion:nil];
}

- (void)imagePickerControllerDidCancel:(UIImagePickerController *)picker {
    NSLog(@"Video picker cancelled");

    // Send cancellation message to Unity
    UnitySendMessage("VideoPickerManager", "OnVideoPickerCancelled", "");

    [picker dismissViewControllerAnimated:YES completion:nil];
}

- (int)getVideoOrientation:(NSURL *)videoURL {
    AVAsset *asset = [AVAsset assetWithURL:videoURL];
    NSArray *tracks = [asset tracksWithMediaType:AVMediaTypeVideo];
    if (tracks.count > 0) {
        AVAssetTrack *videoTrack = tracks[0];
        CGAffineTransform transform = videoTrack.preferredTransform;

        if (transform.a == 0 && transform.b == 1 && transform.c == -1 && transform.d == 0) {
            return 90; // 90 degrees clockwise
        } else if (transform.a == 0 && transform.b == -1 && transform.c == 1 && transform.d == 0) {
            return -90; // 90 degrees counter-clockwise
        } else if (transform.a == -1 && transform.b == 0 && transform.c == 0 && transform.d == -1) {
            return 180; // 180 degrees
        } else {
            return 0; // No rotation
        }
    }
    return 0; // Default to no rotation
}

@end

// Global delegate instance
static VideoPickerDelegate *videoPickerDelegate = nil;

// C functions to be called from Unity
extern "C" {

void _OpenVideoPicker() {
    if (!videoPickerDelegate) {
        videoPickerDelegate = [[VideoPickerDelegate alloc] init];
    }

    UIViewController *rootViewController = UnityGetGLViewController();

    if (rootViewController) {
        if (@available(iOS 14.0, *)) {
            PHPickerConfiguration *config = [[PHPickerConfiguration alloc] init];
            config.selectionLimit = 1;
            config.filter = [PHPickerFilter videosFilter];

            PHPickerViewController *picker = [[PHPickerViewController alloc] initWithConfiguration:config];
            picker.delegate = videoPickerDelegate;

            [rootViewController presentViewController:picker animated:YES completion:nil];
        } else {
            // Fallback for iOS < 14 - use UIImagePickerController without editing
            UIImagePickerController *picker = [[UIImagePickerController alloc] init];
            picker.delegate = videoPickerDelegate;
            picker.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
            picker.mediaTypes = [[NSArray alloc] initWithObjects:(NSString *)kUTTypeMovie, nil];
            picker.videoQuality = UIImagePickerControllerQualityTypeHigh;
            picker.allowsEditing = NO; // Disable editing to prevent trimming

            [rootViewController presentViewController:picker animated:YES completion:nil];
        }
    } else {
        NSLog(@"Error: Could not get root view controller");
    }
}

const char* _GetSelectedVideoPath() {
    // This function could be used for polling if needed
    // For now, return empty string as selection is handled via callbacks
    return "";
}

}