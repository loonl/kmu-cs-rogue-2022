//
//  PNDelegate.h
//  PlayNANOOPlugin_BaseTool
//
//  Created by JONGHYUN LIM on 16/08/2019.
//  Copyright © 2019 JONGHYUN LIM. All rights reserved.
//
@protocol PNDelegate
@optional
-(void)receiveMessage:(NSString *)serviceCode requestCode:(NSString *)requestCode state:(NSString *)state message:(NSString *)message rawData:(NSString *)rawData;
@end

@protocol PNUnitySendMessageDelegate
@optional
-(void)unitySendMessage:(NSString *)functionName message:(NSString *)message;
@end

@protocol PlayNANOOWVDelegate <NSObject>
@required
-(void)onPopupWindowInappCallback:(NSString *)code;
@end

#ifndef PNDelegate_h
#define PNDelegate_h


#endif /* PNDelegate_h */
