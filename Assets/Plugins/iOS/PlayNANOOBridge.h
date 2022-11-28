//
//  PlayNANOOBridge.h
//  PlayNANOOUnity
//
//  Created by LimJongHyun on 2017. 6. 1..
//  Copyright © 2017년 LimJongHyun. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AdSupport/AdSupport.h>
#import <UIKit/UIKit.h>
#import <PlayNANOOPlugin/PlayNANOOPlugin.h>

@interface PlayNANOOBridge : NSObject
 {
     UIViewController *viewController;
     PlayNANOOBridge *bridge;
     PNPlugin *plugin;
 }

-(id)init:(NSString *)gameID serviceKey:(NSString *)serviceKey secretKey:(NSString *)secretKey version:(NSString *)version;
-(void)_pnInit;
-(void)_pnSetDeviceUniqueID:(NSString *)value;
-(void)_pnSetUniqueUserID:(NSString *)value;
-(void)_pnSetNickName:(NSString *)value;
-(void)_pnSetLanguage:(NSString *)value;
-(void)_pnOpenBanner;
-(void)_pnOpenForum;
-(void)_pnOpenForumView:(NSString *)url;
-(void)_pnOpenScreenshot:(NSString *)value;
-(void)_pnHelpDeskOptional:(NSString *)key value:(NSString *)value;
-(void)_pnOpenHelpDesk;
-(void)_pnOpenShare:(NSString *)title message:(NSString *)message;
-(NSString *)_pnCountryCode;
-(NSString *)_pnGetInviteCode;
-(NSString *)_pnGetInviteRequestCode;
@end


