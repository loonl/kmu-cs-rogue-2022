//
//  PNUtils.h
//  PlayNANOOPlugin_BaseTool
//
//  Created by JONGHYUN LIM on 16/08/2019.
//  Copyright © 2019 JONGHYUN LIM. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@interface PNUtils : NSObject
+ (NSString *)urlencode:(NSString *)input;
//+ (NSString*)sha256:(NSString*)clear;
+ (NSString *)base64Data:(NSData *)value;
+ (NSString *)base64SHA256:(NSString *)message secretKey:(NSString *)secretKey;
+ (NSString *)timeStamp;
+ (BOOL)isConnectedNetwork;
+ (NSInteger *) gmtTimeOffset;
+ (NSMutableDictionary *) inviteParse:(NSString *)queryString;
@end

NS_ASSUME_NONNULL_END
