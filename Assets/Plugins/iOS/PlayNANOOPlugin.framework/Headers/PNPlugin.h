//
//  PNPlugin.h
//  PlayNANOOPlugin_BaseTool
//
//  Created by JONGHYUN LIM on 14/08/2019.
//  Copyright © 2019 JONGHYUN LIM. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <PlayNANOOPlugin/PNDelegate.h>
#import <PlayNANOOPlugin/PNUniqueID.h>

NS_ASSUME_NONNULL_BEGIN

@interface PNPlugin : NSObject {
    NSMutableDictionary *optionalDict;
    PNUniqueID *playNANOOUniqueID;
}
@property (nonatomic, strong) id<PNDelegate> delegate;
@property (nonatomic, strong) id<PNUnitySendMessageDelegate> unitySendMessageDelegate;

+(void)setInviteCode:(NSString *)inviteCode;
+(void)setInviteUserRequestCode:(NSString *)inviteUserRequestCode;

-(instancetype)initWithPlugin:(NSString *)uniqueUserID;
-(instancetype)initWithPlugin:(NSString *)uniqueUserID userName:(NSString *)userName;
-(instancetype)initWithPlugin:(NSString *)uniqueUserID userName:(NSString *)userName userLanguage:(NSString *)userLanguage;
-(instancetype)initWithPluginInfo:(NSString *)appID serviceKey:(NSString *)serviceKey secretKey:(NSString *)secretKey version:(NSInteger)version;
-(instancetype)initWithPluginInfo:(NSString *)appID serviceKey:(NSString *)serviceKey secretKey:(NSString *)secretKey version:(NSInteger *)version uniqueUserID:(NSString *)uniqueUserID;
-(instancetype)initWithPluginInfo:(NSString *)appID serviceKey:(NSString *)serviceKey secretKey:(NSString *)secretKey version:(NSInteger *)version uniqueUserID:(NSString *)uniqueUserID userName:(NSString *)userName;
-(instancetype)initWithPluginInfo:(NSString *)appID serviceKey:(NSString *)serviceKey secretKey:(NSString *)secretKey version:(NSInteger *)version uniqueUserID:(NSString *)uniqueUserID userName:(NSString *)userName userLanguage:(NSString *)userLanguage;

// Change UUID
-(void)setUniqueUserID:(NSString *)uuid;

// Change Language
-(void)setUserLanguage:(NSString *)language;
 
// Change Name
-(void)setUserName:(NSString *)name;

-(void)setDeviceUniqueID:(NSString *)uniqueID;

// Access
-(void)accessEvent:(NSString *)requestCode;

// Stats
-(void)statsConnect:(NSString *)requestCode state:(NSString *)state;

// Country Code
-(NSString *)countryCode;

// Coupon
-(void)couponUse:(NSString *)requestCode code:(NSString *)code;
-(void)couponUse:(NSString *)requestCode code:(NSString *)code isCallbackTestMode:(BOOL)isCallbackTestMode;

//Forum
-(void)forumThread:(NSString *)requestCode section:(NSString *)section limit:(NSInteger *)limit;
-(void)forumThread:(NSString *)requestCode limit:(NSInteger *)limit;

// Postbox
-(void)postbox:(NSString *)requestCode;
-(void)postboxItemUse:(NSString *)requestCode uid:(NSString *)uid;
-(void)postboxMultiItemUse:(NSString *)requestCode uids:(NSArray *)uids;
-(void)postboxItemSend:(NSString *)requestCode itemCode:(NSString *)itemCode itemCount:(NSInteger *)itemCount itemExpireDay:(NSInteger *)itemExpireDay itemMessage:(NSString *)itemMessage ;
-(void)postboxItemSend:(NSString *)requestCode itemCode:(NSString *)itemCode itemCount:(NSInteger *)itemCount itemExpireDay:(NSInteger *)itemExpireDay;
-(void)postboxItemFriendSend:(NSString *)requestCode friendUUID:(NSString *)friendUUID itemCode:(NSString *)itemCode itemCount:(NSInteger *)itemCount itemExpireDay:(NSInteger *)itemExpireDay itemMessage:(NSString *)itemMessage ;
-(void)postboxItemFriendSend:(NSString *)requestCode friendUUID:(NSString *)friendUUID itemCode:(NSString *)itemCode itemCount:(NSInteger *)itemCount itemExpireDay:(NSInteger *)itemExpireDay;
-(void)postboxClear:(NSString *)requestCode;
-(void)postboxSubscriptionRegister:(NSString *)requestCode productUUID:(NSString *)productUUID;
-(void)postboxSubscriptionRegister:(NSString *)requestCode productUUID:(NSString *)productUUID offset:(NSInteger *)offset;
-(void)postboxSubscriptionCancel:(NSString *)requestCode productUUID:(NSString *)productUUID;

// InAppPurchase
-(void)iapiOS:(NSString *)requestCode productUID:(NSString *)productUID receipt:(NSString *)receipt currency:(NSString *)currency price:(long)price;
-(void)iapiOS:(NSString *)requestCode productUID:(NSString *)productUID receipt:(NSString *)receipt currency:(NSString *)currency price:(long)price isCallbackTestMode:(BOOL)isCallbackTestMode;

// Storage
-(void)storageSave:(NSString *)requestCode key:(NSString *)key value:(NSString *)value isPrivate:(BOOL)isPrivate;
-(void)storageLoad:(NSString *)requestCode key:(NSString *)key;

// Ranking
-(void)rankingSeason:(NSString *)requestCode uid:(NSString *)uid;
-(void)rankingRecord:(NSString *)requestCode uid:(NSString *)uid score:(long)score data:(NSString *)data;
-(void)rankingList:(NSString *)requestCode uid:(NSString *)uid limit:(NSInteger *)limit;
-(void)rankingList:(NSString *)requestCode uid:(NSString *)uid season:(NSInteger *)season limit:(NSInteger *)limit;
-(void)rankingPersonal:(NSString *)requestCode uid:(NSString *)uid;
-(void)rankingPersonal:(NSString *)requestCode uid:(NSString *)uid season:(NSInteger *)season;

// Invite
-(void)invite:(NSString *)requestCode code:(NSString *)code;

// Friend
-(void)friend:(NSString *)requestCode code:(NSString *)code;
-(void)friendReady:(NSString *)requestCode code:(NSString *)code;
-(void)friendInfo:(NSString *)requestCode code:(NSString *)code;
-(void)friendRequest:(NSString *)requestCode code:(NSString *)code openID:(NSString *)openID;
-(void)friendAccept:(NSString *)requestCode code:(NSString *)code relationshipCode:(NSString *)relationshipCode;
-(void)friendDelete:(NSString *)requestCode code:(NSString *)code relationshipCode:(NSString *)relationshipCode;

// Cache
-(void)cacheExists:(NSString *)requestCode cacheKey:(NSString *)cacheKey;
-(void)cacheGet:(NSString *)requestCode cacheKey:(NSString *)cacheKey;
-(void)cacheMultiGet:(NSString *)requestCode cacheKeys:(NSArray *)cacheKeys;
-(void)cacheSet:(NSString *)requestCode cacheKey:(NSString *)cacheKey cacheValue:(NSString *)cacheValue cacheTTL:(NSInteger *)cacheTTL;
-(void)cacheIncrby:(NSString *)requestCode cacheKey:(NSString *)cacheKey cacheValue:(NSInteger *)cacheValue cacheTTL:(NSInteger *)cacheTTL;
-(void)cacgeDecrby:(NSString *)requestCode cacheKey:(NSString *)cacheKey cacheValue:(NSInteger *)cacheValue cacheTTL:(NSInteger *)cacheTTL;
-(void)cacheDel:(NSString *)requestCode cacheKey:(NSString *)cacheKey;

// Forum
-(void)openBanner:(UIViewController *)viewController webViewDelegate:(id<PNDelegate>)webViewDelegate;
-(void)openForm:(UIViewController *)viewController webViewDelegate:(id<PNDelegate>)webViewDelegate;
-(void)openForumView:(UIViewController *)viewController url:(NSString *)url webViewDelegate:(id<PNDelegate>)webViewDelegate;

// HelpDesk
-(void)helpDeskOptional:(NSString *)key value:(NSString *)value;
-(void)openHelpDesk:(UIViewController *)viewController webViewDelegate:(id<PNDelegate>)webViewDelegate;

// Etc...
-(NSString *)inviteCode;
-(NSString *)inviteUserRequestCode;
-(NSString *)IDFA;

-(void)openShare:(UIViewController *)viewController title:(NSString *)title message:(NSString *)message;
@end

NS_ASSUME_NONNULL_END
