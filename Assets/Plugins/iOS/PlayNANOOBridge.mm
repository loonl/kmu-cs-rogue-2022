//
//  PlayNANOOBridge.m
//  PlayNANOOUnity
//
//  Created by LimJongHyun on 2017. 6. 1..
//  Copyright © 2017년 LimJongHyun. All rights reserved.
//

#import "PlayNANOOBridge.h"

extern "C" UIViewController *UnityGetGLViewController();
extern "C" void UnitySendMessage(const char *, const char *, const char *);

@implementation PlayNANOOBridge

-(id)init:(NSString *)gameID serviceKey:(NSString *)serviceKey secretKey:(NSString *)secretKey version:(NSString *)version {
    self = [super init];
    viewController = UnityGetGLViewController();
    plugin = [[PNPlugin alloc] initWithPluginInfo:gameID serviceKey:serviceKey secretKey:secretKey version:[version integerValue]];

    return self;
}

-(void)_pnInit{}

-(void)_pnSetDeviceUniqueID:(NSString *)value {
    [plugin setDeviceUniqueID:value];
}

-(void)_pnSetUniqueUserID:(NSString *)value {
    [plugin setUniqueUserID:value];
}

-(void)_pnSetNickName:(NSString *)value {
    [plugin setUserName:value];
}

-(void)_pnSetLanguage:(NSString *)value {
    [plugin setUserLanguage:value];
}

-(void)_pnOpenBanner {
    [plugin openBanner:viewController webViewDelegate:(id)self];
}

-(void)_pnOpenForum {
    [plugin openForm:viewController webViewDelegate:(id)self];
}

-(void)_pnOpenForumView:(NSString *)url {
    [plugin openForumView:viewController url:url webViewDelegate:(id)self];
}

-(void)_pnOpenScreenshot:(NSString *)value {}

-(void)_pnHelpDeskOptional:(NSString *)key value:(NSString *)value {
    [plugin helpDeskOptional:key value:value];
}

-(void)_pnOpenHelpDesk {
    [plugin openHelpDesk:viewController webViewDelegate:(id)self];
}

-(void)_pnOpenShare:(NSString *)title message:(NSString *)message {
    [plugin openShare:viewController title:title message:message];
}

-(NSString *)_pnCountryCode {
    return [plugin countryCode];
}

-(NSString *)_pnGetInviteCode {
    return [plugin inviteCode];
}

-(NSString *)_pnGetInviteRequestCode {
    return [plugin inviteUserRequestCode];
}

-(void)onPopupWindowInappCallback:(NSString *)code {
    const char *className = [@"PlayNANOO" UTF8String];
    const char *methodName = [@"OnPopupWindowCallbackCode" UTF8String];
    const char *value = [code UTF8String];
    UnitySendMessage(className, methodName, value);
}

-(void)unitySendMessage:(NSString *)functionName message:(NSString *)message {
    const char *className = [@"PlayNANOO" UTF8String];
    const char *methodName = [functionName UTF8String];
    const char *value = [message UTF8String];
    UnitySendMessage(className, methodName, value);   
}
@end

extern "C" {
    PlayNANOOBridge *bridge;
    
    char* cStringCopy(const char* string)
    {
        if (string == NULL)
            return NULL;
        
        char* res = (char*)malloc(strlen(string) + 1);
        strcpy(res, string);
        
        return res;
    }

    void _pnInit(const char* gameID, const char* serviceKey, const char* secretKey, const char* version) {
        bridge = [[PlayNANOOBridge alloc] init:[NSString stringWithUTF8String:gameID] serviceKey:[NSString stringWithUTF8String:serviceKey] secretKey:[NSString stringWithUTF8String:secretKey] version:[NSString stringWithUTF8String:version]];
    }

    void _pnSetDeviceUniqueID(const char* value) {
        [bridge _pnSetDeviceUniqueID:[NSString stringWithUTF8String:value]];
    }

    void _pnSetUniqueUserID(const char* value) {
        [bridge _pnSetUniqueUserID:[NSString stringWithUTF8String:value]];
    }

    void _pnSetNickName(const char* value) {
        [bridge _pnSetNickName:[NSString stringWithUTF8String:value]];
    }

    void _pnSetLanguage(const char* value) {
        [bridge _pnSetLanguage:[NSString stringWithUTF8String:value]];
    }

    void _pnOpenBanner() {
        [bridge _pnOpenBanner];
    }

    void _pnOpenForum() {
        [bridge _pnOpenForum];
    }

    void _pnOpenForumView(const char* url) {
        [bridge _pnOpenForumView:[NSString stringWithUTF8String:url]];
    }

    void _pnOpenScreenshot(const char* value) {
        [bridge _pnOpenScreenshot:[NSString stringWithUTF8String:value]];
    }

    void _pnHelpDeskOptional(const char* key, const char* value) {
        [bridge _pnHelpDeskOptional:[NSString stringWithUTF8String:key] value:[NSString stringWithUTF8String:value]];
    }

    void _pnOpenHelpDesk() {
        [bridge _pnOpenHelpDesk];
    }

    void _pnOpenShare(const char* title, const char* message) {
        [bridge _pnOpenShare:[NSString stringWithUTF8String:title] message:[NSString stringWithUTF8String:message]];
    }

    char* _pnCountryCode() {
        NSString *countryCode = [bridge _pnCountryCode];
        return cStringCopy([countryCode UTF8String]);
    }
  
    char* _pnGetInviteCode() {
        NSString *inviteCode = [bridge _pnGetInviteCode];
        return cStringCopy([inviteCode UTF8String]);
    }

    char* _pnGetInviteRequestCode() {
        NSString *inviteRequestCode = [bridge _pnGetInviteRequestCode];
        return cStringCopy([inviteRequestCode UTF8String]);
    }
}
