#import <Foundation/Foundation.h>#import <MobileCoreServices/UTCoreTypes.h>
#import <AVFoundation/AVFoundation.h>
#import <CoreLocation/CoreLocation.h>
#import <UIKit/UIKit.h>

#ifdef UNITY_4_0 || UNITY_5_0
#import "iPhone_View.h"
#else
extern UIViewController* UnityGetGLViewController();
#endif

@interface UiOSPermission:NSObject
+ (int)checkCameraPermission;
+ (int)requestCameraPermission;
+ (int)checkGPSPermission;
+ (int)canOpenSettings;
+ (void)openSettings;
+ (int)hasCamera;
@end

@implementation UiOSPermission

+ (int)checkCameraPermission {
	if ([[[UIDevice currentDevice] systemVersion] compare:@"7.0" options:NSNumericSearch] != NSOrderedAscending)
	{
		AVAuthorizationStatus status = [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo];
		if (status == AVAuthorizationStatusAuthorized)
			return 1;
		else if (status == AVAuthorizationStatusNotDetermined )
			return 2;
		else
			return 0;
	}
	
	return 1;
}

+ (int)requestCameraPermission {
	if ([[[UIDevice currentDevice] systemVersion] compare:@"7.0" options:NSNumericSearch] != NSOrderedAscending)
	{
		AVAuthorizationStatus status = [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo];
		if (status == AVAuthorizationStatusAuthorized)
			return 1;
		
		if (status == AVAuthorizationStatusNotDetermined) {
			__block BOOL authorized = NO;
			
			dispatch_semaphore_t sema = dispatch_semaphore_create(0);
			[AVCaptureDevice requestAccessForMediaType:AVMediaTypeVideo completionHandler:^(BOOL granted) {
				authorized = granted;
				dispatch_semaphore_signal(sema);
			}];
			dispatch_semaphore_wait(sema, DISPATCH_TIME_FOREVER);
			
			if (authorized)
				return 1;
			else
				return 0;
		}
			
		return 0;
	}
	
	return 1;
}

+ (int)checkGPSPermission {
	if ([[[UIDevice currentDevice] systemVersion] compare:@"7.0" options:NSNumericSearch] != NSOrderedAscending)
	{
		int status = [CLLocationManager authorizationStatus];

		switch (status) {
			case kCLAuthorizationStatusAuthorizedAlways:
				return 1;
			case kCLAuthorizationStatusAuthorizedWhenInUse:
				return 2;
			case kCLAuthorizationStatusDenied:
				return 3;
			case kCLAuthorizationStatusRestricted:
				return 0;
			default:
				return 1;
		}
	}
	
	return 1;
}

+ (int)canOpenSettings {
	if (&UIApplicationOpenSettingsURLString != NULL)
		return 1;
	else
		return 0;
}

+ (void)openSettings {
	if (&UIApplicationOpenSettingsURLString != NULL)
		[[UIApplication sharedApplication] openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString]];
}

+ (int)hasCamera {
	if ([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera])
		return 1;
	
	return 0;
}

@end

extern "C" int _IOSPermissions_CheckCameraPermission() {
	return [UiOSPermission checkCameraPermission];
}

extern "C" int _IOSPermissions_RequestCameraPermission() {
	return [UiOSPermission requestCameraPermission];
}

extern "C" int _IOSPermissions_CheckGPSPermission() {
	return [UiOSPermission checkGPSPermission];
}

extern "C" int _IOSPermissions_CanOpenSettings() {
	return [UiOSPermission canOpenSettings];
}

extern "C" void _IOSPermissions_OpenSettings() {
	[UiOSPermission openSettings];
}

extern "C" int _IOSPermissions_HasCamera() {
	return [UiOSPermission hasCamera];
}
