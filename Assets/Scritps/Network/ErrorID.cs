/********************************************************************
	created:	2014/12/01
	created:	1:12:2014   14:47
	filename: 	CommonPlatform\Server\Framework\ServerLib\Error\ErrorID.cs
	file path:	CommonPlatform\Server\Framework\ServerLib\Error
	file base:	ErrorID
	file ext:	cs
	author:		史耀力
	
	purpose:	全局错误码定义
*********************************************************************/
using System;

namespace Protocols.Error
{
    /// <summary>
    /// global error define
    /// </summary>
    public enum ErrorID : uint
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,
        /// <summary>
        /// 内部错误
        /// </summary>
        Error_Internal,
        /// <summary>
        /// 玩家不在线
        /// </summary>
        PlayerIsNotOnline,              //玩家不在线
        /// <summary>
        /// 金钱不够
        /// </summary>
        NotEnoughMoney,                 //金钱不够

        /// <summary>
        /// 系统未解锁
        /// </summary>
        SystemIsLocked,

        //////////////////////////////////////////////////////////////////////////
        // login && create

        /// <summary>
        /// 帐号没有登录
        /// </summary>
        AccountNotLogin,

        /// <summary>
        /// 帐号密码非法
        /// </summary>
        AccountPwdInvalid,

        /// <summary>
        /// 内部帐号登录不允许
        /// </summary>
        InternalLoginNotAllow,

        /// <summary>
        /// 创建帐号失败
        /// </summary>
        CreateAccountFailed,

        /// <summary>
        /// 创建角色失败
        /// </summary>
        CreateRoleFailed,

        /// <summary>
        /// 名字长度非法
        /// </summary>
        NameLengthIsInvalid,
        /// <summary>
        /// 密码长度非法
        /// </summary>
        PwdLengthIsInvalid,
        /// <summary>
        /// PUID长度非法
        /// </summary>
        PUIDLengthIsInvalid,
        /// <summary>
        /// 登录密码无效
        /// </summary>
        Error_Password_Check_Failed,
        /// <summary>
        /// 重复登录
        /// </summary>
        Error_Duplicated_Login,
        /// <summary>
        /// 不能找到角色
        /// </summary>
        Error_Cannot_Find_Role,
        /// <summary>
        /// 不能找到帐户
        /// </summary>
        Error_Cannot_Find_Account,
        /// <summary>
        /// 角色已经选择
        /// </summary>
        Error_Already_Selected_Role,
        /// <summary>
        /// 创建帐户已重复
        /// </summary>
        Error_Create_Duplicated_Account,
        /// <summary>
        /// 创建角色已重复
        /// </summary>
        Error_Create_Duplicated_Role,
        /// <summary>
        /// 超过创建角色最大上限
        /// </summary>
        Error_Created_Max_Role_Count,
        /// <summary>
        /// 帐号已被封停
        /// </summary>
        Error_Account_Disable,
        /// <summary>
        /// 帐号登录验证失败
        /// </summary>
        Error_Account_Login_Auth_Failed,

        /// <summary>
        /// 服务器暂未开启
        /// </summary>
        Error_ServerNotOpen,

        /// <summary>
        /// 服务器满员
        /// </summary>
        Error_ServerIsFull,

        //////////////////////////////////////////////////////////////////////////
        // scene
        LineIdIsNotValid,               //游戏线路非法
        /// <summary>
        /// 场景类型非法
        /// </summary>
        SceneTypeIsNotValid,            //场景类型非法
        /// <summary>
        /// 场景非法
        /// </summary>
        SceneIsNotValid,                //场景非法
        /// <summary>
        /// 玩家不在大厅内
        /// </summary>
        PlayerIsNotInLobby,             //玩家不在大厅内
        /// <summary>
        /// 玩家不在场景内
        /// </summary>
        PlayerIsNotInScene,             //玩家不在场景内
        /// <summary>
        /// 场景不存在
        /// </summary>
        SceneDontExist,                 //场景不存在
        /// <summary>
        /// 密码错误
        /// </summary>
        PasswordIsNotRight,             //密码错误
        /// <summary>
        /// 场景人数已满
        /// </summary>
        SceneIsFull,                    //场景人数已满
        SceneIsFullOfStatus,            //场景中某身份玩家人数已满
        /// <summary>
        /// 已经在场景内
        /// </summary>
        AlreadyInScene,                 //已经在场景内
        /// <summary>
        /// 当前状态下不能加入场景
        /// </summary>
        CantJoinInCurState,             //当前状态下不能加入场景
        /// <summary>
        /// 回应过期
        /// </summary>
        ResponseOutOfDate,              //回应过期
        /// <summary>
        /// 权限不够
        /// </summary>
        PrivIsNotEnough,                //权限不够
        /// <summary>
        /// 玩家已经请求加入场景
        /// </summary>
        AlreadyRequestJoin,             //玩家已经请求加入场景
        /// <summary>
        /// 邀请者玩家不在场景中
        /// </summary>
        InviterIsNotInScene,            //邀请者玩家不在场景中
        /// <summary>
        /// 被邀请玩家不在场景中
        /// </summary>
        InviteeIsNotInScene,            //被邀请玩家不在场景中
        /// <summary>
        /// 被邀请玩家已经在场景中
        /// </summary>
        InviteeAlreadyInScene,          //被邀请玩家已经在场景中
        /// <summary>
        /// 当前状态下不能邀请
        /// </summary>
        CantInviteInCurState,           //当前状态下不能邀请
        /// <summary>
        /// 已经发出过邀请
        /// </summary>
        AlreadyInvite,                  //已经发出过邀请
        /// <summary>
        /// 玩家没有邀请过
        /// </summary>
        DidnotInvite,                   //玩家没有邀请过
        /// <summary>
        /// 踢人玩家不在场景内
        /// </summary>
        KickerIsNotInScene,             //踢人玩家不在场景内
        /// <summary>
        /// 被踢玩家不在场景内
        /// </summary>
        KickedIsNotInScene,             //被踢玩家不在场景内
        /// <summary>
        /// 当前状态不能踢人
        /// </summary>
        CantKickInCurState,             //当前状态不能踢人
        /// <summary>
        /// 场景已经达到上限
        /// </summary>
        TooManyScenes,                  //场景已经达到上限
        /// <summary>
        /// 没有空闲场景
        /// </summary>
        NoIdleScene,
        /// <summary>
        /// 状态不合法，不能转换
        /// </summary>
        StateIsNotValid,
        /// <summary>
        /// 玩家没有都点准备
        /// </summary>
        NotAllReady,
        /// <summary>
        /// 身份不合法
        /// </summary>
        StatusIsNotValid,
        /// <summary>
        /// 已经拥有此身份
        /// </summary>
        AlreadyHasStatus,
        /// <summary>
        /// 身份转换失败
        /// </summary>
        TransStatusFail,
        /// <summary>
        /// 身份数量已经达到上限
        /// </summary>
        OverMaxStatusCount,

        //////////////////////////////////////////////////////////////////////////
        // item

        /// <summary>
        /// 道具无效
        /// </summary>
        ItemInvalid,                    //item无效 
        /// <summary>
        /// 无效栏位
        /// </summary>
        ColumnInvalid,                  //无效栏位
        /// <summary>
        /// 无效Grid
        /// </summary>
        GridInvalid,                    //无效Grid
        /// <summary>
        /// Grid被锁
        /// </summary>
        GridLocked,                     //Grid被锁
        /// <summary>
        /// 超过最大Grid数
        /// </summary>
        OverMaxGridCount,               //超过最大Grid数
        /// <summary>
        /// Grid非空
        /// </summary>
        GridNotNull,                    //Grid非空
        /// <summary>
        /// 空间不足
        /// </summary>
        SpaceNotEnough,                 //空间不足
        /// <summary>
        /// Grid为空
        /// </summary>
        GridEmpty,                      //Grid为空
        /// <summary>
        /// 物品数量不足
        /// </summary>
        ItemNotEnough,                  //物品数量不足
        /// <summary>
        /// 物品无法使用
        /// </summary>
        ItemCannotUse,                  //物品无法使用
        /// <summary>
        /// 物品无法装备
        /// </summary>
        ItemCannotEquip,                //物品无法装备
        /// <summary>
        /// 超过存放上限
        /// </summary>
        OverMaxCount,                   //超过存放上限
        /// <summary>
        /// 超过叠加上限
        /// </summary>
        OverMaxStackCount,              //超过叠加上限
        /// <summary>
        /// 不能续费
        /// </summary>
        ItemCannotRenew,                //不能续费

        /// <summary>
        /// 装备互斥的物品
        /// </summary>
        ItemEquipMutex,                //装备互斥的物品

        /// <summary>
        /// 购买的商品是锁住的
        /// </summary>
        ItemBuyLocked,

        //////////////////////////////////////////////////////////////////////////
        // mall

        /// <summary>
        /// 没有找到商品
        /// </summary>
        NotFoundGoods,                  //没有找到商品
        /// <summary>
        /// 商品已经上架，不能再上架
        /// </summary>
        GoodsAlreadyOnSale,             //商品已经上架，不能再上架
        /// <summary>
        /// 商品已经下架，不能再下架
        /// </summary>
        GoodsAlreadyOffSale,            //商品已经下架，不能再下架
        /// <summary>
        /// 商城不存在
        /// </summary>
        MallNotExist,                   //商城不存在
        /// <summary>
        /// 商城暂未开放
        /// </summary>
        MallNotOpen,                    //商城暂未开放
        /// <summary>
        /// 商城已经打开
        /// </summary>
        MallAlreadyOpen,                //商城已经打开
        /// <summary>
        /// 商城已经关闭
        /// </summary>
        MallAlreadyClose,               //商城已经关闭
        /// <summary>
        /// 必须是vip
        /// </summary>
        MustBeVIP,                      //必须是vip
        /// <summary>
        /// 商品不在售卖中
        /// </summary>
        GoodsIsNotOnSale,               //商品不在售卖中
        /// <summary>
        /// 商品售罄
        /// </summary>
        GoodsSoldOut,                   //商品售罄

        /// <summary>
        /// 商城购买商品数量非法
        /// </summary>
        MallBuyGoodsCountInvalid,

        //////////////////////////////////////////////////////////////////////////
        // mail

        /// <summary>
        /// 服务无效,管理器无效或未开启
        /// </summary>
        Mail_ServiceInvalid,
        /// <summary>
        /// 收件箱不存在
        /// </summary>
        Mail_BoxNotFound,
        /// <summary>
        /// 邮件未找到
        /// </summary>
        Mail_MailItemNotFound,
        /// <summary>
        /// 邮件已过期
        /// </summary>
        Mail_MailItemExpired,
        /// <summary>
        /// 邮件标题为空
        /// </summary>
        Mail_MailTitleEmpty,
        /// <summary>
        /// 邮件内容为空
        /// </summary>
        Mail_MailContentEmpty,
        /// <summary>
        /// 发件者无效
        /// </summary>
        Mail_SenderInvalid,
        /// <summary>
        /// 附件无效
        /// </summary>
        Mail_AttacherInvalid,
        /// <summary>
        /// 没有足够的权限
        /// </summary>
        Mail_NotEnoughRights,
        /// <summary>
        /// 未知邮件
        /// </summary>
        Mail_UnknownMailItem,
        /// <summary>
        /// 收件人无效，包括收件人被过滤或者通过过滤但实际投递失败  
        /// </summary>
        Mail_ReceiverInvalid,
        /// <summary>
        /// 附件已领取
        /// </summary>
        Mail_AttacherAlreadyDrawn,
        /// <summary>
        /// 领取附件失败
        /// </summary>
        Mail_AttacherDrawFailed,
        /// <summary>
        /// 收件箱为空
        /// </summary>
        Mail_BoxIsNull,

        /// <summary>
        /// 收件箱已满
        /// </summary>
        Mail_BoxIsFull,

        //////////////////////////////////////////////////////////////////////////
        ///ScoreRule
        Failed,

        // level
        /// <summary>
        /// 角色无效
        /// </summary>
        ActorInvalid,                    //actor无效
        /// <summary>
        /// 关卡无效
        /// </summary>
        LevelInvalid,                    //level无效
        /// <summary>
        /// 分支无效
        /// </summary>
        BranchInvalid,                  //branch无效
        /// <summary>
        /// 关卡日限次数用尽
        /// </summary>
        LevelLimitationReached,         //关卡日限次数用尽
        /// <summary>
        /// 无效日限状态
        /// </summary>
        InvalidLimitationState,         //无效关卡日限状态

        /// <summary>
        /// 过关次数已满
        /// </summary>
        LevelCountIsFull,

        /// <summary>
        /// 请求事件Id和当前事件Id不匹配
        /// </summary>
        LevelEventIdMismatch,
        //////////////////////////////////////////////////////////////////////////
        ///pk
        MatchFailed,                    //配对失败

        NotEnoughPkCount,               //参赛次数不够

        NotEnoughLevel,                 //等级不足

        SkillCooldown,                  //技能冷却时间

        OverMaxBuyTimes,                //超过购买最大次数

        OverMaxPkTimes,                 //超过pk最大次数

        //////////////////////////////////////////////////////////////////////////
        // gm

        /// <summary>
        /// 没有gm权限
        /// </summary>
        Gm_Authority,

        //////////////////////////////////////////////////////////////////////////
        // filter

        /// <summary>
        /// 包含脏字
        /// </summary>
        Filter_CheckFailed,

        //////////////////////////////////////////////////////////////////////////
        ///attr
        FortuneNotEnough,               //财产不够

        DiamondNotEnough,               //钻石不够

        GoldNotEnough,                  //金币不够

        StaminaNotEnough,               //体力不够

        NotEnoughPkPoint,               //兑换代币不足

        Error_Duplicated_RoleName,      //人名重复

        ExploredCountNotEnough,         //探险次数不够

        Error_Cannot_Find_Session,

        PlayerAttrTypeDeltaNotAllowed,  // 指定类型属性不能扣除例如string类型

        PlayerAttrDeltaNotOwner,        // 指定属性不是属于本地的

        PlayerAttrDeltaNotEnough,       // 扣除属性不足

        PlayerAttrRemoteDeltaIsOwner,    // 远程设置的属性属于自己

        PlayerAttrRemoteDeltaNotSameOwner,    // 远程扣除的属性不是相同归属 

        PlayerAttrNameInvalid,          // 非法的属性名

        //////////////////////////////////////////////////////////////////////////
        ///subtitle
        NotEnoughSendSubtitleCount,     //发送弹幕次数不够


        //////////////////////////////////////////////////////////////////////////
        ///manufacture
        DrawingPaperNotEnough,           //图纸不够

        ManufactureInvalidActionType,    // 非法的制作操作类型

        ManufactureInvalidRecipeId,      // 配方ID非法

        ManufactureNoDrawPaper,          // 制作没有图纸

        ManufactureMaterialNotEnough,    // 制作材料不足

        ManufactureDecomposeItemNotEnough,    // 分解物品不足

        ManufactureDecomposeScoreNotEnough,   // 制作分解积分不足

        //////////////////////////////////////////////////////////////////////////
        ///treasureBox
        TreasureBoxFragMentNotEnough,    //四叶草碎片不够
        TreasureBoxCoolDown,             //正在冷却
        TreasureBoxDataIsNull,           //宝箱数量不够
        TreasureBoxTokenNotEnough,       //四叶草不够


        //////////////////////////////////////////////////////////////////////////
        ///FashionMatch 时尚搭配赛错误码
        FashionMatchNotOpen,                         //比赛未开始
        FashionMatchRankNotAvaliable,                //排行榜暂未开放
        FashionMatchAlreadyVoteFriend,               //已经投票过给此好友了
        FashionMatchAlreadyVoteSelf,                 //已经投票过给自己了
        FashionMatchTotalVoteTimesOverflow,          //累计投票数达到最大值
        FashionMatchTotalVoteTimesNotEnough,         //累计的投票数不够消耗了
        FashionMatchPlayerNotAttendMatch,            //玩家没有参赛
        FashionMatchInvalidVoteType,                 //非法的时尚赛投票类型
        FashionMatchInvalidVoteSelf,                 //自投是角色ID不匹配
        FashionMatchInvalidVoteFriend,               //投的好友ID并不是自己的好友
        FashionMatchVotesNotEnough,                  //投票权不够了
        FashionMatchNotEnoughAttenders,              //没有足够的参赛者

        PraiseSearchPlayerInvalid,       //搜索玩家点赞无效
        LeaderboardIndexInvalid,         //索引无效
        FlowerIdInvalid,                 //鲜花ID无效
        FlowerIsLocked,                  //鲜花未解锁
        TokenNotEnough,                  //代币不足
        FlowerCountNotEnough,            //鲜花数量不够

        HasDrawed,                       //已经领取
        NotInTime,                       //不在领奖时间

        //////////////////////////////////////////////////////////////////////////
        LevelIsLocked,                   //关卡没解锁

        //////////////////////////////////////////////////////////////////////////
        ///redeem code
        RedeemCodeIsused,               //兑换码已使用
        RedeemCodeInvalid,              //兑换码无效
        RedeemCodeTypeIsused,           //已使用同类型兑换码
        RedeemCodeTimeout,              //兑换码不在有效期内
		
		//////////////////////////////////////////////////////////////////////////
        //pay
        
        /// <summary>
        /// 非法订单状态
        /// </summary>
        InvalidOrderStat,

        /// <summary>
        /// 更新订单状态失败
        /// </summary>
        Fail2UpdateOrderStat,

        /// <summary>
        /// 非法订单数量信息
        /// </summary>
        InvalidOrderAmount,

        /// <summary>
        /// 非法订单分发
        /// </summary>
        InvalidOrderDistribute,

        /// <summary>
        /// 交易信息不正确
        /// </summary>
        InvalidTradeInfo,

        /// <summary>
        /// 重复交易
        /// </summary>
        DuplicateTrade,

        /// <summary>
        /// 无效交易
        /// </summary>
        InvalidTrade,

        /// <summary>
        /// 反查失败
        /// </summary>
        Fail2CheckBack,

        /// <summary>
        /// 支付服务异常
        /// </summary>
        PayServiceError,

        /// <summary>
        /// 订单支付请求失败
        /// </summary>
        PayRequestFailed,

        /// <summary>
        /// 支付订单校验失败
        /// </summary>
        PayBillCheckFailed,

        /// <summary>
        /// 获取pfkey失败
        /// </summary>
        PayGetPfKeyFailed,

        //////////////////////////////////////////////////////////////////////////
        // Friend

        /// <summary>
        /// 好友是自己
        /// </summary>
        FriendIsSelf,

        /// <summary>
        /// 没有该好友
        /// </summary>
        FriendNotFound,

        /// <summary>
        /// 已经是好友
        /// </summary>
        AlreadyFriends,

        /// <summary>
        /// 对方好友申请列表已满
        /// </summary>
        ApplyIsFull,

        /// <summary>
        /// 对方好友列表已满
        /// </summary>
        HisFriendIsFull,

        /// <summary>
        /// 自己好友列表已满
        /// </summary>
        FriendIsFull,

        /// <summary>
        /// 对方礼物已满
        /// </summary>
        GiftIsFull,

        /// <summary>
        /// 对方不在申请列表里
        /// </summary>
        ApplyNotFound,

        /// <summary>
        /// 已经申请
        /// </summary>
        AlreadyApply,

        /// <summary>
        /// 好友送礼CD
        /// </summary>
        FriendGiftInCD,

        /// <summary>
        /// 没有好友赠送的礼物
        /// </summary>
        FriendGiftEmpty,

        /// <summary>
        /// 不能给自己发体力
        /// </summary>
        FriendGiftSelfNotAllow,

        /// <summary>
        /// 没有可以赠送体力的好友
        /// </summary>
        FriendNoFriendToSendGift,


        ////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 没有该首冲礼包
        /// </summary>
        FRNoSuchGift,


        /// <summary>
        /// 首充礼包已经领取
        /// </summary>
        FRGiftClosed,

        /// <summary>
        /// 该首充礼包还不可领取
        /// </summary>
        FRGiftNotAcceptable,

        /// <summary>
        /// 送礼时间未到
        /// </summary>
        SendGiftTimeLimit,

        /// <summary>
        /// 领取体力超过上限
        /// </summary>
        DrawStaminaLimit,

        /// <summary>
        /// 没有该礼物
        /// </summary>
        GiftNotFound,

        /// <summary>
        /// 累积礼包已经领取过了
        /// </summary>
        ARGiftClosed,

        /// <summary>
        /// 图纸重复购买
        /// </summary>
        PaperDuplicated,

        /// <summary>
        /// 补签次数已满
        /// </summary>
        NotEnoughDiamondSign,

        /////////////////////////////////////////////
        // vitality活跃度

        /// <summary>
        /// 没有足够的活跃度
        /// </summary>
        NotEnoughVitality,

        /// <summary>
        /// 已经领取过了此奖励
        /// </summary>
        AlreadyGetVitalityReward,

        /// <summary>
        /// 尚未拥有该头像
        /// </summary>
        PortraitNotOwned,

        /// <summary>
        /// 尚未拥有该头像边框
        /// </summary>
        PortraitFrameNotOwned,

        /// <summary>
        /// 姓名已经被使用
        /// </summary>
        NameUsed,

        /// <summary>
        /// 等级不够,不可以发送好友邮件
        /// </summary>
        Mail_NotEnoughLevel,

        /// <summary>
        /// 标题过长
        /// </summary>
        Mail_TitleTooLong,

        /// <summary>
        /// 正文过长
        /// </summary>
        Mail_ContentTooLong,

        /// <summary>
        /// 对方不是好友
        /// </summary>
        Mail_TargetIsNotFriend,

        //////////////////////////////////////////////////////////////////////////
        // Chat

        /// <summary>
        /// 非法频道
        /// </summary>
        Chat_InvalidChannel,

        /// <summary>
        /// 等级过低
        /// </summary>
        Chat_LevelTooLow,

        /// <summary>
        /// 禁言时间
        /// </summary>
        Chat_InForbiddenTime,

        /// <summary>
        /// 每日聊天次数耗尽
        /// </summary>
        Chat_RunoutDailyChat,

        /// <summary>
        /// 聊天内容重复
        /// </summary>
        Chat_SameMessage,

        /// <summary>
        /// 聊天内容过长
        /// </summary>
        Chat_MessageToLong,

        /// <summary>
        /// 聊天过于频繁
        /// </summary>
        Chat_TooFrequent,

        /// <summary>
        /// 聊天目标非法
        /// </summary>
        Chat_InvalidTarget,

        /// <summary>
        /// 喇叭购买钻石不够
        /// </summary>
        Chat_NotEnoughDiamond4Speaker,

        /// <summary>
        /// 目标对象不在线
        /// </summary>
        Chat_TargetNotOnline,

        /// <summary>
        /// 频道已满
        /// </summary>
        Chat_ChannelIsFull,

        /// <summary>
        /// 非法的套装ID
        /// </summary>
        Chat_InvalidSuitID,

        /// <summary>
        /// 黑名单已满
        /// </summary>
        Friend_BlackIsFull,

        /// <summary>
        /// 已经在黑名单中
        /// </summary>
        Friend_AlreadyInBlack,

        /// <summary>
        /// 不在黑名单中
        /// </summary>
        Friend_NotInBlack,

        //////////////////////////////////////////////////////////////////////////////
        //Guild公会

        /// <summary>
        /// 创建公会角色等级不够
        /// </summary>
        Guild_CreateRoleLevelNotEnough,

        /// <summary>
        /// 创建公会费用不足
        /// </summary>
        Guild_CreateCostNotEnough,

        /// <summary>
        /// 创建公会名字已经被占用
        /// </summary>
        Guild_CreateNameIdOccupied,

        /// <summary>
        /// 找不到指定的公会
        /// </summary>
        Guild_NotExist,

        /// <summary>
        /// 不是公会成员
        /// </summary>
        Guild_NotMembers,

        /// <summary>
        /// 不是公会管理人员
        /// </summary>
        Guild_NotManagers,

        /// <summary>
        /// 不是会长，没有权限
        /// </summary>
        Guild_NotChairman,

        /// <summary>
        /// 不是副会长，不能辞职
        /// </summary>
        Guild_NotViceChairman,

        /// <summary>
        /// 公会成员已满
        /// </summary>
        Guild_MembersFull,

        /// <summary>
        /// 重复申请加入相同的公会
        /// </summary>
        Guild_ApplySameGuild,

        /// <summary>
        /// 被批准成员不在申请列表中
        /// </summary>
        Guild_PlayerNotInApplies,

        /// <summary>
        /// 玩家申请的公会数量达到上限
        /// </summary>
        Guild_PlayerAppliesExceedLimit,

        /// <summary>
        /// 加入的公会不在申请列表里
        /// </summary>
        Guild_JoinGuildNotInApplies,

        /// <summary>
        /// 被批准成员已经有公会了
        /// </summary>
        Guild_PlayerAlreadyInGuild,

        /// <summary>
        /// 调整相同的职位了
        /// </summary>
        Guild_AdjustSamePost,

        /// <summary>
        /// 副会长人数已满
        /// </summary>
        Guild_ExceedViceChairManCount,

        /// <summary>
        /// 精英成员已满
        /// </summary>
        Guild_ExceedSeniorMemberCount,

        /// <summary>
        /// 非法的调整职位
        /// </summary>
        Guild_InvalidAdjustPost,

        /// <summary>
        /// 还有公会成员不能解散
        /// </summary>
        Guild_NotAllowDisbandDueMembers,

        /// <summary>
        /// 公会管理者(会长副会长)不能退会
        /// </summary>
        Guild_ManagersNotAllowQuit,

        /// <summary>
        /// 被踢者不是公会成员
        /// </summary>
        Guild_BekickedNotMembers,

        /// <summary>
        /// 管理者不能直接被踢
        /// </summary>
        Guild_ManagersNotAllowKicked,

        /// <summary>
        /// 获取章节信息错误
        /// </summary>
        Guild_ListChapterFailed,

        /// <summary>
        /// 获取关卡信息错误
        /// </summary>
        Guild_ListLevelFailed,

        /// <summary>
        /// 公会关卡未打开
        /// </summary>
        Guild_LevelNotOpen,

        /// <summary>
        /// 公会关卡已经完成
        /// </summary>
        Guild_LevelAlreadyFinished,

        /// <summary>
        /// 没有足够的训练次数了
        /// </summary>
        Guild_NotEnoughTrainCount,

        /// <summary>
        /// 关卡没有通过
        /// </summary>
        Guild_LevelNotPass,

        /// <summary>
        /// 关卡是F通过，不能快速通关
        /// </summary>
        Guild_LevelPassGradeF,

        /// <summary>
        /// 公会关卡非法
        /// </summary>
        Guild_LevelInvalid,

        /// <summary>
        /// 超过公会训练的可购买上限
        /// </summary>
        Guild_ExceedTrainBuyLimit,

        /// <summary>
        /// 超过公会日常操作的限制
        /// </summary>
        Guild_ExceedGuildEventLimit,

        /// <summary>
        /// 公会Buff等级已满级
        /// </summary>
        Guild_BuffLevelReachMax,

        /// <summary>
        /// 没有足够的公会代币
        /// </summary>
        Guild_NotEnoughGuildCoins,

        /// <summary>
        /// 没有足够的工会研究水晶
        /// </summary>
        Guild_NotEnoughGuildCrystal,

        /// <summary>
        /// 还没有拥有此会徽
        /// </summary>
        Guild_NotOwnEmblem,

        /// <summary>
        /// 只能转给副会长
        /// </summary>
        Guild_TransferMustViceChairMan,

        /// <summary>
        /// 在冷却期内不能转让
        /// </summary>
        Guild_TransferInCdTime,

        /// <summary>
        /// 关卡奖励已经不存在了
        /// </summary>
        Guild_LevelRewardNotExist,

        /// <summary>
        /// 不能领取此关卡奖励
        /// </summary>
        Guild_LevelRewardCannotDraw,

        /// <summary>
        /// 不能领取训练奖励
        /// </summary>
        Guild_TrainRewardCannotDraw,

        /// <summary>
        /// 公会未进入TopN榜单
        /// </summary>
        Guild_NotInTopN,

        /// <summary>
        /// 已经领过奖励
        /// </summary>
        Guild_RewardAlreadyDraw,

        /// <summary>
        /// 公会申请CD中
        /// </summary>
        Guild_ApplyInCD,

        /// <summary>
        /// 非法的事件类型
        /// </summary>
        Guild_InvalidEventType,

        /// <summary>
        /// 免费许愿CD中
        /// </summary>
        Guild_FreeWishCD,

        /// <summary>
        /// 公会当前冷却周期内已经寻求过帮助了
        /// </summary>
        Guild_AlreadySeekHelp,

        /// <summary>
        /// 帮助已满
        /// </summary>
        Guild_HelpIsFull,

        /// <summary>
        /// 许愿树开放时间未到
        /// </summary>
        Guild_WishTreeOpenTimeIsUp,

        /// <summary>
        /// 许愿树未开放采摘
        /// </summary>
        Guild_WishTreeNotOpen,

        /// <summary>
        /// 许愿树已经开放
        /// </summary>
        Guild_WishTreeAlreadyOpen,

        /// <summary>
        /// 公会许愿树果子耗尽
        /// </summary>
        Guild_WishTreeFruitExhaust,

        /// <summary>
        /// 手套不足
        /// </summary>
        Guild_WishTreePickChanceNotEnough,

        /// <summary>
        /// 公会任务不存在
        /// </summary>
        Guild_QuestNotExist,

        /// <summary>
        /// 公会任务奖励已经领取
        /// </summary>
        Guild_QuestAlreadtDraw,

        /// <summary>
        /// 公会任务未达成
        /// </summary>
        Guild_QuestNotReach,

        /// <summary>
        /// 工会研究院等级超过当前公会等级
        /// </summary>
        Guild_BuffUpgradeExceedGuildLevel,

        /// <summary>
        /// 工会研究院等级超过最大值
        /// </summary>
        Guild_BuffUpgradeExceedMaxLevel,

        /// <summary>
        /// 许愿树开启，还在冷却时间
        /// </summary>
        Guild_WishTreeOpenInCdTime,

        /// <summary>
        /// 不能帮助自己
        /// </summary>
        Guild_WishCannotHelpSelf,

        /// <summary>
        /// 公会非法的支付类型
        /// </summary>
        Guild_InvalidPayType,

        //////////////////////////////////////////////////////////////////////////////////////////////
        //GuildDB

        /// <summary>
        /// 数据库插入公会创建数据失败
        /// </summary>
        GuildDb_CreateFailed,

        /// <summary>
        /// 更新公会数据库失败
        /// </summary>
        GuildDb_UpdateFailed,

        /// <summary>
        /// 查询公会数据失败
        /// </summary>
        GuildDb_SelectFailed,

        /// <summary>
        /// 查询不到对应id的数据
        /// </summary>
        GuildDb_SelectNoData,

        /// <summary>
        /// 数据库删除失败
        /// </summary>
        GuildDb_DeleteFailed,

        /// <summary>
        /// 只有S级才可以扫荡
        /// </summary>
        Level_GradeTooLow,

        /// <summary>
        /// 非法的商城类型
        /// </summary>
        Mall_InvalidType,

        /// <summary>
        /// 商城物品购买超过限制上限
        /// </summary>
        Mall_GoodsBuyExceedLimit,


        /////////////////////////////////////////////////////////////////////////////////////////////////////
        //Activity 活动

        /// <summary>
        /// 活动暂时不开放
        /// </summary>
        ActivityNotAvailable,

        /// <summary>
        /// 领取时间过期
        /// </summary>
        ActivityDrawExpire,

        /// <summary>
        /// 奖励领取失败
        /// </summary>
        ActivityDrawFailed,

        /// <summary>
        /// 活动项不存在
        /// </summary>
        ActivityItemNotExist,

        /// <summary>
        /// 领取项不匹配
        /// </summary>
        ActivityItemNotMatch,

        /// <summary>
        /// 领取项不能领取，未满足条件
        /// </summary>
        ActivityItemCannotDraw,

        /// <summary>
        /// 活动项已经领取
        /// </summary>
        ActivityItemAlreadyDraw,

        /// <summary>
        /// 活动项已经可以领取
        /// </summary>
        ActivityItemAlreadyCanDraw,

        /// <summary>
        /// 已经首充过了
        /// </summary>
        ActivityRechargeFirstAlready,

        ///////////////////////////////////////////////////////////////////////////////////
        //成就
        /// <summary>
        /// 不存在的成就
        /// </summary>
        AchievementNotExist,

        /// <summary>
        /// 成就未达成
        /// </summary>
        AchievementNotReached,

        /// <summary>
        /// 前置成就未完成
        /// </summary>
        AchievementPreNotFinish,

        ///////////////////////////////////////////////////////////////////////////////////
        //Quest
        /// <summary>
        /// 任务不存在
        /// </summary>
        QuestNotExist,

        /// <summary>
        /// 任务未完成
        /// </summary>
        QuestNotReached,

        ///////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Vip等级不够
        /// </summary>
        VipGradeNotEnough,

        ///////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///  答题id不存在
        /// </summary>
        QuizIdNotExist,

        /// <summary>
        /// 活动未开启
        /// </summary>
        QuizClosed,

        /// <summary>
        /// 题目不是当前题目
        /// </summary>
        QuizIncorrectId,


        /// <summary>
        /// 次数已用完
        /// </summary>
        Level2LimitationReached,

        /// <summary>
        /// 喇叭券不足
        /// </summary>
        Chat_NotEnoughSpeakerTicket,

        /// <summary>
        /// 喇叭券不足
        /// </summary>
        Chat_NotEnoughSceneSpeakerTicket,

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 礼包售卖类型非法
        /// </summary>
        GiftBagSellTypeInvalid,

        /// <summary>
        /// 礼包售卖超过上限
        /// </summary>
        GiftBagSellExceedLimit,

        /// <summary>
        /// 礼包售卖无法赠送
        /// </summary>
        GiftBagSellCannotPresent,
        ////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// cdkey 失效
        /// </summary>
        CDKEY_Expired,

        /// <summary>
        /// cdkey 不正确
        /// </summary>
        CDKEY_InCorrect,

        /// <summary>
        /// cdkey 已使用
        /// </summary>
        CDKEY_Used,

        /// <summary>
        /// 这个批次cdkey已达上限
        /// </summary>
        CDKEY_MaxLimited,

        /// <summary>
        /// 此玩家非此cdkey绑定玩家
        /// </summary>
        CDKEY_IncorrectBindedUser,


        ///////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 超过场景最大人数
        /// </summary>
        Scene_ExceedMaxRoleNum,

        /// <summary>
        /// 没有加入任何房间
        /// </summary>
        Scene_NotJoinAnyScene,

        /// <summary>
        /// 场景不存在
        /// </summary>
        Scene_NotExist,

        /// <summary>
        /// 玩家不在场景内
        /// </summary>
        Scene_RoleNotInScene,

        /// <summary>
        /// 被踢玩家不在场景内
        /// </summary>
        Scene_BeKickedRoleNotInScene,

        /// <summary>
        /// 不是房主
        /// </summary>
        Scene_NotSceneOwner,

        /// <summary>
        /// 不能踢有Vip，并且Vip等级超过我
        /// </summary>
        Scene_CannotKickVipUpRole,

        /// <summary>
        /// 不能踢在小游戏中的角色
        /// </summary>
        Scene_CannotKickInMiniGameRole,

        /// <summary>
        /// 在小游戏中不能被使用技能
        /// </summary>
        Scene_CannotBeUseKillInMiniGame,

        /// <summary>
        /// 修改场景主题CD中
        /// </summary>
        Scene_ChangeThemInCD,

        /// <summary>
        /// 置顶需求变化
        /// </summary>
        Scene_StickyEstimateChange,

        /// <summary>
        /// 房间名已经被占用
        /// </summary>
        Scene_RoomNameIsOccupied,

        /// <summary>
        /// 场景房间已满
        /// </summary>
        Scene_RoomIsFull,

        /// <summary>
        /// 场景幸运值不足
        /// </summary>
        Scene_LuckyNotEnough,

        /// <summary>
        /// 幸运事件已满
        /// </summary>
        Scene_LuckyEventIsFull,

        /// <summary>
        /// 幸运事件掉落失败
        /// </summary>
        Scene_LuckyEventDropFailed,

        /// <summary>
        /// 幸运技能已满
        /// </summary>
        Scene_LuckySkillIsFull,

        /// <summary>
        /// 幸运技能掉落失败
        /// </summary>
        Scene_LuckySkillDropFailed,

        /// <summary>
        /// 派对房间密码错误
        /// </summary>
        Scene_PasswordIncorrect,

        /// <summary>
        /// 派对幸运事件索引已经不存在，可能被别人抢走了
        /// </summary>
        Scene_SceneEventIndexNotExist,

        /// <summary>
        /// 玩家已经在派对中了
        /// </summary>
        Scene_RoleAlreadyInScene,

        /// <summary>
        /// 没有可加入的房间
        /// </summary>
        Scene_NoRoomCanJoin,

        /// <summary>
        /// 置顶卡数量不足
        /// </summary>
        Scene_StickyCardNotEnough,

        /// <summary>
        /// 随机技能的索引不存在了，可能被其他人抢走了
        /// </summary>
        Scene_RandSkillIndexNotExist,

        /// <summary>
        /// 随机技能的角色ID和请求的角色Id不一致
        /// </summary>
        Scene_RandSkillTargetRoleNotMatch,

        /// <summary>
        /// 派对活力值已满
        /// </summary>
        Scene_VitalityIsFull,

        /// <summary>
        /// 切换相同的房间
        /// </summary>
        Scene_SwitchTheSameScene,

        /// <summary>
        /// 密码超长
        /// </summary>
        Scene_PasswordExceedLength,

        /// <summary>
        /// 当前派对技能状态非法
        /// </summary>
        Scene_RoleSkillStatusInvalid,

        /// <summary>
        /// 已经邀请对方加入公会
        /// </summary>
        Scene_AlredyInviteGuild,

        /// <summary>
        /// 派对技能使用还未开启
        /// </summary>
        Scene_SkillUseNotOpen,

        /// <summary>
        /// 派对使用技能等级不足
        /// </summary>
        Scene_UseSkillLevelLimit,

        /// <summary>
        /// 派对技能，守护者专属
        /// </summary>
        Scene_UseSkillGuardianLimit,

        /// <summary>
        /// 派对技能使用未知的限制类型
        /// </summary>
        Scene_UseSkillUnknowLimitType,

        /// <summary>
        /// 派对技能使用必须为好友关系
        /// </summary>
        Scene_UseSkillFriendLimit,

        /// <summary>
        /// 派对技能使用必须拥有物品限制
        /// </summary>
        Scene_UseSkillOwnItemLimit,
        /////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 新的一天已经到了，次数已刷新
        /// </summary>
        Level2_QuickPass_Max,


        /// <summary>
        /// 全区维护中
        /// </summary>
        Dir_AllZone_Offline,

        /// <summary>
        /// 钻石翻翻翻下一层无效
        /// </summary>
        DiamondRetrun_NextFloorIdNotVariable,

        /// <summary>
        /// 找不到宝箱配置数据
        /// </summary>
        GiftBag_DataIsNull,

        /// <summary>
        /// 入会不满3天,不能为捐献公会
        /// </summary>
        Guild_JoinTimeTooShort,

        /// <summary>
        /// 不允许复数使用
        /// </summary>
        Item_CannotMultiUse,

        /// <summary>
        /// 积分非负,无法清0
        /// </summary>
        MGA_ScoreNotBelowZero,



        // 任务
        /// <summary>
        /// 非当前任务id
        /// </summary>
        Task_InvalidId,

        /// <summary>
        /// 任务未完成
        /// </summary>
        Task_NotFinished,

        ////////////////////////////////////////////////////////////////////////////////////////////
        // pet宠物系统

        /// <summary>
        /// 宠物捐赠已达上限
        /// </summary>
        Pet_DonateReachMax,

        /// <summary>
        /// 宠物捐赠未达上限
        /// </summary>
        Pet_DonateNotReachMax,

        /// <summary>
        /// 非法的捐赠类型
        /// </summary>
        Pet_DonateInvalidType,

        /// <summary>
        /// 没有宠物
        /// </summary>
        Pet_None,

        /// <summary>
        /// 没有该宠物
        /// </summary>
        Pet_DbIdErr,

        /// <summary>
        /// 没有该动作
        /// </summary>
        Pet_ActionNone,

        /// <summary>
        /// 没有动作配置
        /// </summary>
        Pet_ActionCfgErr,

        /// <summary>
        /// 没有动作熟练度已满
        /// </summary>
        Pet_ActionExpEnough,

        /// <summary>
        /// 物品不存在或数量不足
        /// </summary>
        Pet_ItemNotExistsOrEnough,

        /// <summary>
        /// 物品配置错误
        /// </summary>
        Pet_ItemCfgErr,

        /// <summary>
        /// 物品无法使用
        /// </summary>
        Pet_ItemUnavailable,

        /// <summary>
        /// 属性不足，物品无法使用
        /// </summary>
        Pet_ItemUnavailable_AttrNotEnough,

        /// <summary>
        /// 属性已满，物品无法使用
        /// </summary>
        Pet_ItemUnavailable_AttrIsFull,

        /// <summary>
        /// 已达使用上限，物品无法使用，使用更高级物品
        /// </summary>
        Pet_ItemUnavailable_Limit,

        /// <summary>
        /// 物品无法使用，每次只能使用一个
        /// </summary>
        Pet_ItemUnavailable_One,

        /// <summary>
        /// 宠物物品不是宠物财富
        /// </summary>
        Pet_ItemIsNotPetFortune,

        /// <summary>
        /// 宠物建筑等级已满
        /// </summary>
        Pet_BuildLevelReachMax,

        /// <summary>
        /// 宠物建筑强化等级已满
        /// </summary>
        Pet_BuildStrengthLevelReachMax,

        /// <summary>
        /// 宠物唯一Id重复
        /// </summary>
        Pet_IdDuplicate,

        /// <summary>
        /// 宠物建筑还未拥有
        /// </summary>
        Pet_BuildingNotOwned,

        /// <summary>
        /// 宠物建筑升级超过小屋等级
        /// </summary>
        Pet_BuildingUpgradeOverHourseLevel,

        /// <summary>
        /// 不是活动状态的宠物Id
        /// </summary>
        Pet_NotActivePetId,

        /// <summary>
        /// 宠物活动位置超过了最大值
        /// </summary>
        Pet_ActivePositionExceed,

        /// <summary>
        /// 宠物不存在
        /// </summary>
        Pet_NotExist,

        /// <summary>
        /// 宠物非法
        /// </summary>
        Pet_IdInvalid,

        /// <summary>
        /// 宠物建筑已经存在
        /// </summary>
        Pet_BuildingExist,

        /// <summary>
        /// 收集的建筑产出为空
        /// </summary>
        Pet_BuildGatherEmpty,

        /// <summary>
        /// 自己战斗中
        /// </summary>
        Pet_TradeSelfUnderTrade,

        /// <summary>
        /// 对手战斗中
        /// </summary>
        Pet_TradeOpponentUnderTrade,

        /// <summary>
        /// 对手免战
        /// </summary>
        Pet_TradeOpponentRefusalToFight,

        /// <summary>
        /// 没有出战的宠物
        /// </summary>
        Pet_TradeNoActivityPet,

        /// <summary>
        /// 宠物精力不足
        /// </summary>
        Pet_TradeEnergyNotEnough,

        /// <summary>
        /// 贸易正在进行中
        /// </summary>
        Pet_TradeInGameStatus,

        /// <summary>
        /// 贸易还未进入准备状态
        /// </summary>
        Pet_TradeNotPrepare,

        /// <summary>
        /// 宠物贸易非法的位置
        /// </summary>
        Pet_TradeInvalidPos,

        /// <summary>
        /// 宠物贸易位置已经关闭，战斗结束
        /// </summary>
        Pet_TradePosClose,

        /// <summary>
        /// 宠物贸易存在更强大的属性
        /// </summary>
        Pet_TradeAttrMorePower,

        /// <summary>
        /// 宠物处于幼崽状态
        /// </summary>
        Pet_IsChild,

        /// <summary>
        /// 宠物生病或者宠物休眠
        /// </summary>
        Pet_SickOrSleep,

        /// <summary>
        /// 非贸易进行中，不能使用物品
        /// </summary>
        Pet_TradeNotInGameStatus,

        /// <summary>
        /// 购买贸易次数已达最大值
        /// </summary>
        Pet_TradeBuyEnterMax,

        /// <summary>
        /// 购买精力次数已达最大值
        /// </summary>
        Pet_BuyEnergyMax,

        /// <summary>
        /// 宠物悬赏配置错误
        /// </summary>
        Pet_PostRewardCfgErr,

        /// <summary>
        /// 宠物悬赏不存在
        /// </summary>
        Pet_PostRewardNotExists,

        /// <summary>
        /// 宠物悬赏过期
        /// </summary>
        Pet_PostRewardOvertime,

        /// <summary>
        /// 宠物悬赏全服限制
        /// </summary>
        Pet_PostRewardServerLimit,

        /// <summary>
        /// 宠物悬赏个人限制
        /// </summary>
        Pet_PostRewardPersonLimit,

        /// <summary>
        /// 贸易物品数量非法
        /// </summary>
        Pet_TradeItemCountInvalid,

        /// <summary>
        /// 贸易对手物品数量不足
        /// </summary>
        Pet_TradeOppItemCountNotEnough,

        /// <summary>
        /// 贸易物品数量不足
        /// </summary>
        Pet_TradeItemCountNotEnough,

        /// <summary>
        /// 宠物贸易没有对手
        /// </summary>
        Pet_TradeNoOpponent,

        /// <summary>
        /// 探索需要订单板等级
        /// </summary>
        Pet_ExploreNeedOrderboardLevel,

        /// <summary>
        /// 贸易物品超过最大值
        /// </summary>
        Pet_TradeItemCountOverMax,

        /// <summary>
        /// 非法的对手
        /// </summary>
        Pet_TradeInvalidOpponent,

        /// <summary>
        /// 没有刷新次数
        /// </summary>
        Pet_ExchangeRefreshNoTimes,

        /// <summary>
        /// 当前套装未解锁
        /// </summary>
        Pet_ExchangeCurrentSuitLocked,

        /// <summary>
        /// 当前套装是最后一套
        /// </summary>
        Pet_ExchangeTheLastOne,

        /// <summary>
        /// 此服饰未解锁
        /// </summary>
        Pet_ExchangeClothesLocked,

        /// <summary>
        /// 此服饰未出现
        /// </summary>
        Pet_ExchangeClothesUnDisplayed,

        /// <summary>
        /// 此服饰分数已满
        /// </summary>
        Pet_ExchangeClothesScoreFull,

        /// <summary>
        /// 游戏已结束
        /// </summary>
        Pet_ExchangeGameFinished,

        /// <summary>
        /// 游戏失败，不加分
        /// </summary>
        Pet_ExchangeGameFailed,

        /// <summary>
        /// 没有更多的暴击次数
        /// </summary>
        Pet_ExchangeNoCritCount,

        /// <summary>
        /// 暴击失败
        /// </summary>
        Pet_ExchangeCritFailed,

        /// <summary>
        /// 已经购买自动点击
        /// </summary>
        Pet_ExchangeIsAutoClick,

        //////////////////////////////////////////////////////////////////////////
        // 倾慕之颜

        /// <summary>
        /// 体力不足 
        /// </summary>
        InstA_NotEnoughStamina,

        /// <summary>
        /// 该关卡未配置奖励
        /// </summary>
        InstA_RewardNotOpen,

        /// <summary>
        /// 该关卡尚未通过 
        /// </summary>
        InstA_RewardNotAvailable,
        
        /// <summary>
        /// 已经领过了
        /// </summary>
        InstA_RewardClosed,

        /// <summary>
        /// 所有关卡均已通过
        /// </summary>
        InstA_AllLevelPassed,

        /// <summary>
        /// 服饰已经使用过
        /// </summary>
        InstA_ClothUsed,
        
        /// <summary>
        /// 体力已满,无法购买
        /// </summary>
        InstA_StaminaIsFull,

        //////////////////////////////////////////////////////////////////////////
        // 宠物 - 探索

        /// <summary>
        /// 无效的订单序列号
        /// </summary>
        PetOrder_InvalidIndex,

        /// <summary>
        /// 错误的订单状态
        /// </summary>
        PetOrder_WrongStatus,

        /// <summary>
        /// 力量不够
        /// </summary>
        PetOrder_NotEnoughStr,

        /// <summary>
        /// 智力不够
        /// </summary>
        PetOrder_NotEnoughInt,

        /// <summary>
        /// 宠物体力不足 
        /// </summary>
        Pet_NotEnoughStamina,

        /// <summary>
        /// 工作状态不对
        /// </summary>
        PetOrder_WrongWorkStatus,

        /// <summary>
        /// 无空闲订单位
        /// </summary>
        PetOrder_NoFreeOrderIndex,

        /// <summary>
        /// 等级不够
        /// </summary>
        PetOrder_NotEnoughLevel,

        /// <summary>
        /// 宠物精力不足 
        /// </summary>
        Pet_NotEnoughEnergy,

        //////////////////////////////////////////////////////////////////////////
        //AAS防沉迷

        /// <summary>
        /// 身份证非法
        /// </summary>
        AAS_IDIllegal,
    }
}
