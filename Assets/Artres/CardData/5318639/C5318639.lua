local C5318639={}

function C5318639.InitInfo(card)
	card:SetCardTypeByString("魔法");
	card:SetName("旋风");
	card:SetMagicTypeByString("速攻");
	card:SetEffectInfo("①：可以以场上的1张魔法陷阱卡为对象发动。将那张卡破坏。");
end

function C5318639.CanLaunchEffect(card)
	local myMagicTrapCardArea = card:GetDuelCardScript():GetOwner():GetMagicTrapCardArea();
	local opponentMagicTrapCardArea = card:GetDuelCardScript():GetOwner():GetOpponentPlayer():GetMagicTrapCardArea();

	for i=0,myMagicTrapCardArea.Length-1 do
		if(myMagicTrapCardArea[i]~=nil and myMagicTrapCardArea[i]~=card)then
			return true;
		end
	end
	for i=0,opponentMagicTrapCardArea.Length-1 do
		if(opponentMagicTrapCardArea[i]~=nil and opponentMagicTrapCardArea[i]~=card)then
			return true;
		end
	end

	return false;
end

function C5318639.LaunchEffect(card)
	
	CS.Assets.Script.GameManager.ShowMessage("请选择被破坏的魔法陷阱卡！")

	local myMagicTrapCardArea = card:GetDuelCardScript():GetOwner():GetMagicTrapCardArea();
	local opponentMagicTrapCardArea = card:GetDuelCardScript():GetOwner():GetOpponentPlayer():GetMagicTrapCardArea();

	for i=0,myMagicTrapCardArea.Length-1 do
		if(myMagicTrapCardArea[i]~=nil and myMagicTrapCardArea[i]~=card)then
			myMagicTrapCardArea[i]:GetDuelCardScript():SetClickCallback(card,C5318639.ChooseCardCallback);
		end
	end
	for i=0,opponentMagicTrapCardArea.Length-1 do
		if(opponentMagicTrapCardArea[i]~=nil and opponentMagicTrapCardArea[i]~=card)then
			opponentMagicTrapCardArea[i]:GetDuelCardScript():SetClickCallback(card,C5318639.ChooseCardCallback);
		end
	end
end

function C5318639.ChooseCardCallback(launchEffectCard,chooseCard)
	if((chooseCard:GetCardType()~=CS.Assets.Script.Card.CardType.Magic and chooseCard:GetCardType()~=CS.Assets.Script.Card.CardType.Trap) or launchEffectCard==chooseCard)then
		return
	end

	local myMagicTrapCardArea = launchEffectCard:GetDuelCardScript():GetOwner():GetMagicTrapCardArea();
	local opponentMagicTrapCardArea = launchEffectCard:GetDuelCardScript():GetOwner():GetOpponentPlayer():GetMagicTrapCardArea();

	for i=0,myMagicTrapCardArea.Length-1 do
		if(myMagicTrapCardArea[i]~=nil and myMagicTrapCardArea[i]~=launchEffectCard)then
			myMagicTrapCardArea[i]:GetDuelCardScript():RemoveClickCallback();
		end
	end
	for i=0,opponentMagicTrapCardArea.Length-1 do
		if(opponentMagicTrapCardArea[i]~=nil and opponentMagicTrapCardArea[i]~=launchEffectCard)then
			opponentMagicTrapCardArea[i]:GetDuelCardScript():RemoveClickCallback();
		end
	end

	local moveCardToTombEffectProcess = CS.Assets.Script.Duel.EffectProcess.MoveCardToTombEffectProcess(chooseCard,CS.Assets.Script.Duel.EffectProcess.MoveCardToTombType.Effect, launchEffectCard:GetDuelCardScript():GetOwner());
    launchEffectCard:GetDuelCardScript():GetOwner():AddEffectProcess(moveCardToTombEffectProcess);
end

return C5318639