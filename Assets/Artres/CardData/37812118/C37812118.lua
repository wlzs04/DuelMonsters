local C37812118={}

function C37812118.InitInfo(card)
	card:SetCardTypeByString("魔法");
	card:SetName("圣杯A");
	card:SetMagicTypeByString("通常");
	card:SetEffectInfo("抛1次硬币。出现正面的场合，自己从卡组抽2张卡。出现反面的场合，对手从卡组抽2张卡。");
end

function C37812118.CanLaunchEffect(card)
	return true;
end

function C37812118.LaunchEffect(card)
	local tossCoinEffectProcess = CS.Assets.Script.Duel.EffectProcess.TossCoinEffectProcess(card,false,C37812118.TossCoinCallBack,CS.Assets.Script.Duel.EffectProcess.CoinType.Unknown, card:GetOwner());
    card:GetOwner():AddEffectProcess(tossCoinEffectProcess);
end

function C37812118.TossCoinCallBack(card,selectCoinType,resultCoinType)
	if(resultCoinType==CS.Assets.Script.Duel.EffectProcess.CoinType.Front)then
		local drawCardEffectProcess = CS.Assets.Script.Duel.EffectProcess.DrawCardEffectProcess(2,CS.Assets.Script.Duel.EffectProcess.DrawCardType.Effect,card:GetOwner());
		card:GetOwner():AddEffectProcess(drawCardEffectProcess);
	elseif(resultCoinType==CS.Assets.Script.Duel.EffectProcess.CoinType.Back)then
		local drawCardEffectProcess = CS.Assets.Script.Duel.EffectProcess.DrawCardEffectProcess(2,CS.Assets.Script.Duel.EffectProcess.DrawCardType.Effect,card:GetOwner():GetOpponentPlayer());
		card:GetOwner():GetOpponentPlayer():AddEffectProcess(drawCardEffectProcess);
	end
end

return C37812118