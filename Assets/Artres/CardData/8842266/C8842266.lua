local C8842266={}

function C8842266.InitInfo(card)
	card:SetCardTypeByString("魔法");
	card:SetName("隐居者的猛毒药");
	card:SetMagicTypeByString("速攻");
	card:SetEffectInfo("①：可以从以下的效果中选择1个发动。●自己恢复1200LP。●给予对手800伤害。");
end

function C8842266.CanLaunchEffect(card)
	return true;
end

function C8842266.LaunchEffect(card)
	local totalStringList = CS.Assets.Script.Helper.XLuaHelper.CreateStringList();
	totalStringList:Add("●自己恢复1200LP。");
	totalStringList:Add("●给予对手800伤害。");
	card:GetDuelScene():ShowEffectSelectPanel(card,totalStringList,C8842266.ChooseIndexCallback);
end

function C8842266.ChooseIndexCallback(launchEffectCard,index)
	if(index==0)then
		local changeLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ChangeLifeEffectProcess(-1200, CS.Assets.Script.Duel.EffectProcess.ChangeLifeType.Effect, launchEffectCard:GetOwner())
		launchEffectCard:GetOwner():AddEffectProcess(changeLifeEffectProcess);
	elseif(index==1)then
		local changeLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ChangeLifeEffectProcess(800, CS.Assets.Script.Duel.EffectProcess.ChangeLifeType.Effect, launchEffectCard:GetOwner():GetOpponentPlayer())
		launchEffectCard:GetOwner():GetOpponentPlayer():AddEffectProcess(changeLifeEffectProcess);
	end
end

return C8842266