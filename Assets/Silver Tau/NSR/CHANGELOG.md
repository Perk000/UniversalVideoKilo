# Changelog
All notable changes to this package are documented in this file.

## [1.8.2]
### Updated
- Native Screen Recorder;
- NSR Core Libraries;
- NSR Libraries;
- Improved presentation handling (safe rotation);
- Stability improvements for Native Screen Recorder initialization;
- Utilities;
- Documentation;
### Fixed
- Popover/Share presentation issue after screen orientation change (iOS);
- Minor bugs;

## [1.8.1]
### Added:
- Extended support for older versions of Unity;
- Improvements for Unity 6+ versions;
- dSYM;
### Updated
- Graphic Provider & Subsystem;
- Universal Video Recorder;
- Editor Recorder;
- NSR Libraries;
- Utilities;
- Documentation;
### Fixed
- Warning about missing dSYM in Xcode;
- Minor bugs;

## [1.8.0]
### Added:
- Safe GPU Blit for Watermark;
- Dynamic adaptation of the video frame rate to the device and app performance;
### Updated
- Universal Video Recorder;
- Optimizing video recording;
- Frame encoding method;
- NSR Libraries;
- Watermark;
- Documentation;
### Fixed
- Flickering or rendering artifacts when using watermark (Metal);
- Frame rate change for certain devices;
- Minor bugs;

## [1.7.3]
### Updated
- Universal Video Recorder;
- Custom Frame Processor;
- Utility Libraries;
- NSR Libraries;
- Documentation;
### Fixed
- Minor bugs;

## [1.7.2]
### Added:
- Watermark;
- Custom Frame Processor;
- An example of using a watermark;
- An example of a multi-scene recording;
- Record audio for the Editor Recorder;
- Events for working with video recording frames;
- Support for GPU & CPU operations for Editor Recorder;
- Support for asynchronous operations for Editor Recorder;
- A new asynchronous solution for a webcam (WebCamUITextureAsync);
### Updated
- Universal Video Recorder;
- Editor Recorder;
- NSR Core Libraries;
- Expanded range of settings and options;
- Documentation;
- Examples;
### Fixed
- Fixed a bug with no vibration after using the Microphone() function;
- Minor bugs;

## [1.7.1]
### Added:
- Audio Receiver component;
- Additional options for working with audio;
- New features for working with a microphone;
### Updated
- Universal Video Recorder;
- Editor Recorder;
- NSR Core Libraries;
- NSR Utility Libraries;
- Audio recording subsystem;
- Expanded range of settings and options;
- Documentation;
- Examples;
### Fixed
- Microphone echo when recording audio with additional audio sources;
- Minor bugs;

## [1.7.0]
### Added:
- Custom video resolution option;
- Ability to change the path to save to the gallery (Android);
- New Actions for tracking the status of a video recording;
- Ability to customize the video resolution for any device orientation (portrait / landscape);
- Video Preview Component (VideoPlayer);
- Video Preview Component (Handheld);
- Video timer sample (Text);
### Updated
- Universal Video Recorder;
- Editor Recorder;
- NSR Core Libraries;
- NSR Utility Libraries;
- Gallery & Gallery Extensions;
- Preview Video Manager;
- Color spaces (Gamma and Linear);
- Expanded range of settings and options;
- Optimization of the plugin;
- sRGB & HDR options;
- Library structure;
- Documentation;
- Examples;
### Fixed
- Minor bugs;

## [1.6.0]
### Added:
- Scripting Define Symbols (NSR_MICROPHONE_DISABLE, NSR_CAMERA_DISABLE);
- The ability to manage permissions;
- Share Extensions;
- Gallery Extensions;
- File Saver;
### Updated
- Universal Video Recorder;
- NSR Core Libraries;
- Library structure;
- Graphic Provider & Subsystem;
- Shared Graphic;
- Shared Graphic (Extended functionality of the path to the output file);
- The recording length of a separate audio file has been increased;
- Microphone Audio (Input & Recorder);
- Separate audio file;
- Permission Helper;
- Documentation;
- Examples;
### Fixed
- Minor bugs;

## [1.5.4]
### Added:
- Dynamic camera changes when recording video (all platforms & Editor);
- An example of a scene with a dynamic camera change;
- New options for saving video and audio files;
### Updated
- Universal Video Recorder;
- NSR Core Libraries;
- Editor recorder;
- Documentation;
- Examples;
### Fixed
- Minor bugs;

## [1.5.3]
### Added:
- Dynamic camera changes when recording video (all platforms & Editor);
- An example of a scene with a dynamic camera change;
- New options for saving video and audio files;
### Updated
- Universal Video Recorder;
- NSR Core Libraries;
- Editor recorder;
- Documentation;
- Examples;
### Fixed
- Minor bugs;

## [1.5.2]
### Added:
- Callback function (UnityAction) for the Gallery utility;
- Verification of the plugin at application launch;
### Updated
- Optimization of the plugin;
- Universal Video Recorder;
- NSR Core Libraries;
- Editor recorder;
- Examples;
### Fixed
- Fixed a delay during plugin initialization;
- Minor bugs;

## [1.5.1]
### Updated
- Universal Video Recorder;
- Windows encoder;
- Video corders;
- NSR Core Libraries;
- Editor recorder;
### Fixed
- Re-record video in the Unity editor bugs;
- Recording without a microphone for the Windows platform bugs;
- Minor bugs;

## [1.5.0]
### Added
- Microphone Audio Recorder;
- Record individual audio files;
- Separate an audio file from a recorded video;
- Scene example for recording separate video files;
- New options for recording video;
### Updated
- Simplifying the process of adding a custom path for video and audio files;
- Updated interface;
- Optimization of the plugin;
- Optimization of video recording on Windows and macOS platforms;
- Reduced load when recording video;
- Updating the first black frame of the video;
- Expanded range of settings and options;
- Universal Video Recorder;
- File Manager;
- Video Corders;
- NSR Core Libraries;
- Editor Encoders;
- Examples;
- Documentation;
### Fixed
- Minor bugs;

## [1.4.4]
### Updated
- Universal Video Recorder;
- Video Corders;
- NSR Libraries;
### Fixed
- Minor bugs;

## [1.4.3]
### Updated
- Universal Video Recorder;
- Video Corders;
- NSR Libraries;
- Plugin validation function;
- Optimization of the plugin;
### Fixed
- Minor bugs;

## [1.4.2]
### Added
- Advanced video settings;
### Updated
- Universal Video Recorder;
- Video Corders;
- NSR Libraries;
- The "Bitrate" property for video settings;
### Fixed
- Minor bugs;

## [1.4.1]
### Added
- Advanced sound settings;
- The "Sample rate" property for audio settings;
- The "Channel count" property for audio settings;
- The "Bitrate" property for audio settings;
- New examples;
### Updated
- Universal Video Recorder;
- Video Corders;
- NSR Libraries;
- Plugin validation function;
- The function of skipping frames during video recording;
- Graphic Provider (Screenshot & Image system);
- Optimization of the plugin;
### Fixed
- Video recording on Windows and macOS platforms;
- Auto-detection of bitrate;
- Screen recording with UI layer (Screen Space - Camera or World Space);
- Minor bugs;

## [1.4.0]
### Added
- Transparent video recording sample (Editor Recorder);
- Utility for saving photos and videos to the Gallery (iOS, iPadOS, Android);
- Utility for sharing any files, videos, images and e.t.c (iOS, iPadOS, Android);
- File manager utility;
- The function of skipping frames during video recording;
- File management & Share & Save to gallery - examples;
### Updated
- Universal Video Recorder;
- Editor Encoders;
- NSR Libraries;
- Editor Corder;
- Examples of interaction with the plugin;
- Optimization of the plugin;
### Fixed
- Recording video in Unity Editor for Windows;
- Minor bugs;

## [1.3.13]
### Added
- Transparent video recording (Editor Recorder);
- Transparent video formats .mp4, .webm (Editor Recorder);
### Updated
- Different resolutions (LR, VHS, LD, SD, HD, FHD, QHD, UHD);
- Universal Video Recorder;
- Video resolution settings;
- Editor H264 Encoder;
- Editor VP8 Encoder;
- Editor Corder;
- Video Recorders;
### Fixed
- Minor bugs;

## [1.3.12]
### Added
- Different resolutions (SD, HD, FHD, QHD, UHD);
- Automatically detects safe video resolution;
### Updated
- Universal Video Recorder;
- Video resolution settings;
- Resolution divide function;
- Audio Settings;
- Video Recorders;
- A simple camera example;
### Fixed
- Minor bugs;

## [1.3.11]
### Added
- Editor VP8 Encoder;
### Updated
- Universal Video Recorder;
- Video Recorders;
- WebCam Texture;
- Editor H264 Encoder;
### Fixed
- Editor Recorders;
- Minor bugs;

## [1.3.10]
### Updated
- Graphic Provider (Screenshot & Image system);
- Editor Corder;
### Fixed
- Editor Recorders;
- Graphic Provider (XR);
- Minor bugs;

## [1.3.9]
### Added
- RenderTexture video recording settings;
- RenderTexture video recording sample;
### Updated
- Graphic Provider (Screenshot & Image system);
- Universal Video Recorder;
- Video Recorders;
- Editor Corder;
- Optimization of the plugin;
- Documentation;
### Fixed
- Minor bugs;

## [1.3.8]
### Added
- HDR (high dynamic range) support;
### Updated
- Editor Corder;
- Video Recorders;
- Universal Video Recorder;
- Graphic Provider (Screenshot & Image system);
### Fixed
- Minor bugs;

## [1.3.7]
### Added
- Editor Corder;
- Graphic Provider (Screenshot & Image system);
- Example of a graphic provider;
### Updated
- Video Recorders;
- Universal Video Recorder;
### Fixed
- Minor bugs;
- Corder bugs;

## [1.3.6]
### Added
- Async Tasks;
- NSR validate function;
### Updated
- Video Recorders;
- Microphone solution (Windows/macOS);
### Fixed
- Minor bugs;

## [1.3.5]
### Added
- A simple camera example;
- Image effects;
- Shaders;
- Audio;
### Updated
- Video Recorders;
- Microphone solution;
- Optimization of the plugin;
### Fixed
- Minor bugs;

## [1.3.4]
### Added
- Audio settings;
- Audio Input type;
- Record params;
### Updated
- Video Recorders;
- Audio Input;
- Microphone solution;
- Audio system;
- Optimization of the plugin;
### Fixed
- Minor bugs;
- Build bugs;

## [1.3.3]
### Updated
- Native Video Recorder;
- Frameworks;
- Internal Corder;
- iOS version support;
### Fixed
- Minor bugs;

## [1.3.2]
### Added
- HEVC codec;
### Updated
- Universal Video Recorder;
- Internal Corder;
- Method of dividing video resolution;
### Fixed
- Minor bugs;

## [1.3.1]
### Added
- Dynamic scene (sample);
- Hold and Record (sample);
### Updated
- Universal Video Recorder;
- Android library;
### Fixed
- Minor bugs;

## [1.3.0]
### Added
- Windows support;
- macOS support;
### Updated
- Universal Video Recorder;
- Optimization of the plugin;
- Examples;
### Fixed
- Minor bugs;

## [1.2.1]
### Added
- Automatically pause/resume video recording during program focus/pause;
- Custom frame rate;
### Updated
- Universal Video Recorder;
### Fixed
- Pause/Resume video recording function;
- Microphone echo;
- Optimization of the plugin;
- Minor bugs;

## [1.2.0]
### Added
- Pause/Resume video recording function;
### Updated
- Rename output video file function;
- Save output video file function;
### Fixed
- Minor bugs;
 
## [1.1.1]
### Added
- Updated NSR - Screen Recorder;
- Updated Corder;
 
## [1.1.0]
### Added
- Added Android support;
- Added Universal Screen Recorder;
- Updated NSR - Screen Recorder;

## [1.0.0]
### Added
 - Release;