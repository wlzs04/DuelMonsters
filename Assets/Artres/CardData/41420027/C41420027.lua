local C41420027={}

function C41420027.InitInfo(card)
	card:SetCardTypeByString("陷阱");
	card:SetName("神之宣告");
	card:SetMagicTypeByString("反击");
	card:SetEffectInfo("①：可以支付一半LP发动以下的效果。●魔法、陷阱卡发动时可以发动。使其发动无效并破坏。●自己或者对手召唤、反转召唤或特殊召唤怪兽之际可以发动。使那个无效并将那些怪兽破坏。");
end

function C41420027.CanLaunchEffect(card)
	--在召唤、发动魔法、陷阱卡牌时才能发动。
	local currentChainEffectProcess = card:GetDuelScene():GetCurrentChainEffectProcess(card);
	if(currentChainEffectProcess~=nil and 
		(currentChainEffectProcess:GetType()==typeof(CS.Assets.Script.Duel.EffectProcess.CallMonsterEffectProcess) or
		currentChainEffectProcess:GetType()==typeof(CS.Assets.Script.Duel.EffectProcess.LaunchEffectEffectProcess)))then
		return true;
	end

	return false;
end

function C41420027.Cost(card)
	local currentChainEffectProcess = card:GetDuelScene():GetCurrentChainEffectProcess();
	--先将自己的生命值减半
	local myLifeValue = card:GetOwner():GetLife();
	card:GetOwner():ChangeLife(-myLifeValue/2);
	currentChainEffectProcess:CostFinish()
end

function C41420027.LaunchEffect(card)
	local currentChainEffectProcess = card:GetDuelScene():GetCurrentChainEffectProcess(card);
	if(currentChainEffectProcess~=nil and currentChainEffectProcess:GetType()==typeof(CS.Assets.Script.Duel.EffectProcess.CallMonsterEffectProcess))then
		local calledMonster = currentChainEffectProcess:GetCalledMonster();
		local moveCardToTombEffectProcess = CS.Assets.Script.Duel.EffectProcess.MoveCardToTombEffectProcess(calledMonster,CS.Assets.Script.Duel.EffectProcess.MoveCardToTombType.Effect, calledMonster:GetOwner());
		calledMonster:GetOwner():AddEffectProcess(moveCardToTombEffectProcess);
		return;
	end
	if(currentChainEffectProcess~=nil and currentChainEffectProcess:GetType()==typeof(CS.Assets.Script.Duel.EffectProcess.LaunchEffectEffectProcess))then
		local launchCard = currentChainEffectProcess:GetLaunchCard();
		currentChainEffectProcess:SetBeDisabled(true);
		local moveCardToTombEffectProcess = CS.Assets.Script.Duel.EffectProcess.MoveCardToTombEffectProcess(launchCard,CS.Assets.Script.Duel.EffectProcess.MoveCardToTombType.Effect, launchCard:GetOwner());
		launchCard:GetOwner():AddEffectProcess(moveCardToTombEffectProcess);
		return;
	end
end

return C41420027