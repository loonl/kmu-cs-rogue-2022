//
//  PNUniqueID.h
//  PlayNANOOPlugin
//
//  Created by JONGHYUN LIM on 2021/07/30.
//  Copyright © 2021 JONGHYUN LIM. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@interface PNUniqueID : NSObject
- (NSString *)getID;
- (void)setID:(NSString *)deviceID;
@end

NS_ASSUME_NONNULL_END
