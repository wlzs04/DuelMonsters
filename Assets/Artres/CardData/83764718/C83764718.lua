local C83764718={}

function C83764718.InitInfo(card)
	card:SetCardTypeByString("魔法");
	card:SetName("死者苏生");
	card:SetMagicTypeByString("通常");
	card:SetEffectInfo("①：可以以自己或者对手墓地的1只怪兽为对象发动。将那只怪兽特殊召唤到自己的场上。");
end

function C83764718.CanLaunchEffect(card)
	local myTombCardList = card:GetDuelCardScript():GetOwner():GetTombCards();
	local opponentTombCardList = card:GetDuelCardScript():GetOwner():GetOpponentPlayer():GetTombCards();
	
	for i=0,myTombCardList.Count-1 do
		if(myTombCardList[i]:GetCardType()==CS.Assets.Script.Card.CardType.Monster)then
			return true;
		end
	end
	for i=0,opponentTombCardList.Count-1 do
		if(opponentTombCardList[i]:GetCardType()==CS.Assets.Script.Card.CardType.Monster)then
			return true;
		end
	end
	
	return false;
end

function C83764718.LaunchEffect(card)
	local myTombCardList = card:GetDuelCardScript():GetOwner():GetTombCards();
	local opponentTombCardList = card:GetDuelCardScript():GetOwner():GetOpponentPlayer():GetTombCards();
	
	local totalCardList = CS.Assets.Script.Helper.XLuaHelper.CreateCardBaseList();
	for i=0,myTombCardList.Count-1 do
		totalCardList:Add(myTombCardList[i])
	end
	for i=0,opponentTombCardList.Count-1 do
		totalCardList:Add(opponentTombCardList[i])
	end

	card:GetDuelCardScript():GetDuelScene():ShowCardList(totalCardList,"请选择要召唤的怪兽:",false,card,C83764718.ChooseCardCallback);
end

function C83764718.ChooseCardCallback(launchEffectCard,card)
	if(card:GetCardType()~=CS.Assets.Script.Card.CardType.Monster)then
		return
	end
	card:GetDuelCardScript():GetDuelScene():HideCardList(false);
	local callMonsterEffectProcess = CS.Assets.Script.Duel.EffectProcess.CallMonsterEffectProcess(card,CS.Assets.Script.Card.CardGameState.Unknown, launchEffectCard:GetDuelCardScript():GetOwner());
    launchEffectCard:GetDuelCardScript():GetOwner():AddEffectProcess(callMonsterEffectProcess);
end

return C83764718