#import <UIKit/UIKit.h>
#import <MobileCoreServices/MobileCoreServices.h>
#import "UnityInterface.h"

// Global variables to store Unity callback objects
extern "C" {
    void UnitySendMessage(const char* obj, const char* method, const char* msg);
}

@interface VideoPickerDelegate : NSObject <UIImagePickerControllerDelegate, UINavigationControllerDelegate>
@property (nonatomic, strong) UIViewController *pickerController;
@end

@implementation VideoPickerDelegate

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
        UIImagePickerController *picker = [[UIImagePickerController alloc] init];
        picker.delegate = videoPickerDelegate;
        picker.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
        picker.mediaTypes = [[NSArray alloc] initWithObjects:(NSString *)kUTTypeMovie, nil];
        picker.videoQuality = UIImagePickerControllerQualityTypeHigh;

        [rootViewController presentViewController:picker animated:YES completion:nil];
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