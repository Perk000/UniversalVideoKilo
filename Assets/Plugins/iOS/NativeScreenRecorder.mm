#import <UIKit/UIKit.h>
#import <AVFoundation/AVFoundation.h>
#import <ReplayKit/ReplayKit.h>
#import <Photos/Photos.h>
#import "UnityInterface.h"

// Global variables
static RPScreenRecorder *screenRecorder = nil;
static NSString *outputFilePath = nil;

// C functions to be called from Unity
extern "C" {

void _StartScreenRecording(const char* filePath) {
    if (!screenRecorder) {
        screenRecorder = [RPScreenRecorder sharedRecorder];
    }

    outputFilePath = [NSString stringWithUTF8String:filePath];

    // Check if screen recording is available
    if (![screenRecorder isAvailable]) {
        UnitySendMessage("VideoRecorder", "OnRecordingError", "Screen recording not available");
        return;
    }

    // Start recording
    [screenRecorder startRecordingWithHandler:^(NSError *error) {
        if (error) {
            NSString *errorMsg = [NSString stringWithFormat:@"Failed to start recording: %@", error.localizedDescription];
            UnitySendMessage("VideoRecorder", "OnRecordingError", [errorMsg UTF8String]);
        } else {
            UnitySendMessage("VideoRecorder", "OnRecordingStarted", "");
            NSLog(@"Screen recording started");
        }
    }];
}

void _StopScreenRecording() {
    if (!screenRecorder || ![screenRecorder isRecording]) {
        UnitySendMessage("VideoRecorder", "OnRecordingError", "No active recording to stop");
        return;
    }

    [screenRecorder stopRecordingWithHandler:^(RPPreviewViewController *previewViewController, NSError *error) {
        if (error) {
            NSString *errorMsg = [NSString stringWithFormat:@"Failed to stop recording: %@", error.localizedDescription];
            UnitySendMessage("VideoRecorder", "OnRecordingError", [errorMsg UTF8String]);
        } else {
            UnitySendMessage("VideoRecorder", "OnRecordingStopped", "");

            // Save recording to file if path is specified
            if (outputFilePath) {
                // Note: In a full implementation, you would need to handle the previewViewController
                // or use alternative methods to save the recording
                NSLog(@"Recording stopped. Preview available. Output path: %@", outputFilePath);
            }
        }
    }];
}

bool _IsRecording() {
    return screenRecorder && [screenRecorder isRecording];
}

void _SaveToCameraRoll(const char* filePath) {
    NSString *path = [NSString stringWithUTF8String:filePath];
    if ([[NSFileManager defaultManager] fileExistsAtPath:path]) {
        [[PHPhotoLibrary sharedPhotoLibrary] performChanges:^{
            [PHAssetChangeRequest creationRequestForAssetFromVideoAtFileURL:[NSURL fileURLWithPath:path]];
        } completionHandler:^(BOOL success, NSError *error) {
            if (success) {
                UnitySendMessage("VideoRecorder", "OnSavedToCameraRoll", "");
            } else {
                NSString *errorMsg = [NSString stringWithFormat:@"Failed to save to camera roll: %@", error.localizedDescription];
                UnitySendMessage("VideoRecorder", "OnRecordingError", [errorMsg UTF8String]);
            }
        }];
    } else {
        UnitySendMessage("VideoRecorder", "OnRecordingError", "File not found for saving to camera roll");
    }
}

}