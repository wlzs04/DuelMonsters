local C2130625={}

function C2130625.InitInfo(card)
	card:SetCardTypeByString("陷阱");
	card:SetName("白衣天使");
	card:SetTrapTypeByString("通常");
	card:SetEffectInfo("自己因战斗或者卡的效果受到伤害时可以发动。自己恢复1000LP。自己的墓地有「白衣の天使／白衣天使」存在的场合，再恢复那个张数×500LP。");
end

function C2130625.CanLaunchEffect(card)
	local currentChainEffectProcess = card:GetDuelScene():GetCurrentChainEffectProcess();
	if(currentChainEffectProcess~=nil and currentChainEffectProcess:GetType()==typeof(CS.Assets.Script.Duel.EffectProcess.ChangeLifeEffectProcess))then
		if(card:GetOwner()==currentChainEffectProcess:GetOwnerPlayer() and currentChainEffectProcess:GetChangeLifeValue()<0)then
			return true;
		end
	end
	return false;
end

function C2130625.LaunchEffect(card)
	local myTombCardList = card:GetOwner():GetTombCards();
	local sameNameCardCount = 0;
	for i=0,myTombCardList.Count-1 do
		if(myTombCardList[i]:GetName()==card:GetName())then
			sameNameCardCount=sameNameCardCount+1;
		end
	end
	local changeLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ChangeLifeEffectProcess(1000+sameNameCardCount*500, CS.Assets.Script.Duel.EffectProcess.ChangeLifeType.Effect, card:GetOwner())
    card:GetOwner():AddEffectProcess(changeLifeEffectProcess);
end

return C2130625