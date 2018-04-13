
LinkDelegate = {};
local this = LinkDelegate;

-- 所有的LINK点击事件都会传入到这个lua内进行处理
function LinkDelegate.OnAction(linkInfo)
	print(linkInfo);
end