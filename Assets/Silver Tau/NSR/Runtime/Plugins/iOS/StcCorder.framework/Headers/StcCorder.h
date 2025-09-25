#pragma once

#include <stdint.h>

#ifdef __cplusplus
    #define BRIDGE extern "C"
#else
    #define BRIDGE
#endif

#ifdef _WIN64
    #define EXPORT __declspec(dllexport)
#else
    #define EXPORT
    #define APIENTRY
#endif


#pragma region --Types--
struct NCMediaRecorder;
typedef struct NCMediaRecorder NCMediaRecorder;
typedef void (*NCRecordingHandler) (void* context, const char* path);
#pragma endregion


#pragma region --IMediaRecorder--
BRIDGE EXPORT void APIENTRY NCMediaRecorderFrameSize (
    NCMediaRecorder* recorder,
    int32_t* outWidth,
    int32_t* outHeight
);
BRIDGE EXPORT void APIENTRY NCMediaRecorderCommitFrame (
    NCMediaRecorder* recorder,
    const uint8_t* pixelBuffer,
    int64_t timestamp
);
BRIDGE EXPORT void APIENTRY NCMediaRecorderCommitSamples (
    NCMediaRecorder* recorder,
    const float* sampleBuffer,
    int32_t sampleCount,
    int64_t timestamp
);
BRIDGE EXPORT void APIENTRY NCMediaRecorderFinishWriting (
    NCMediaRecorder* recorder,
    NCRecordingHandler completionHandler,
    void* context
);
#pragma endregion


#pragma region --Constructors--
BRIDGE EXPORT void APIENTRY NCCreateMP4Recorder (
    const char* path,
    int32_t width,
    int32_t height,
    float frameRate,
    int32_t sampleRate,
    int32_t channelCount,
    int32_t videoBitRate,
    int32_t keyframeInterval,
    int32_t audioBitRate,
    NCMediaRecorder** recorder
);
BRIDGE EXPORT void APIENTRY NCCreateHEVCRecorder (
    const char* path,
    int32_t width,
    int32_t height,
    float frameRate,
    int32_t sampleRate,
    int32_t channelCount,
    int32_t videoBitRate,
    int32_t keyframeInterval,
    int32_t audioBitRate,
    NCMediaRecorder** recorder
);
BRIDGE EXPORT void APIENTRY NCCreateGIFRecorder (
    const char* path,
    int32_t width,
    int32_t height,
    float frameDuration,
    NCMediaRecorder** recorder
);
#pragma endregion
