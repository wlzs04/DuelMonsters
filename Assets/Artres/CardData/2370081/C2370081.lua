local C2370081={}

function C2370081.InitInfo(card)
	card:SetCardTypeByString("魔法");
	card:SetName("钢之甲壳");
	card:SetMagicTypeByString("装备");
	card:SetEffectInfo("只有水属性怪兽才能装备。装备怪兽的攻击力上升400点，守备力下降200点。");
end

function C2370081.CanLaunchEffect(card)
	local monsterCard = card:GetEquidMonster();
	if(monsterCard~=nil)then
		return false;
	end
	local myMonsterCardArea = card:GetDuelCardScript():GetOwner():GetMonsterCardArea();
	local opponentMonsterCardArea = card:GetDuelCardScript():GetOwner():GetOpponentPlayer():GetMonsterCardArea();

	for i=0,myMonsterCardArea.Length-1 do
		if(myMonsterCardArea[i]~=nil and (myMonsterCardArea[i]:GetCardGameState()==CS.Assets.Script.Card.CardGameState.FrontAttack or myMonsterCardArea[i]:GetCardGameState()==CS.Assets.Script.Card.CardGameState.FrontDefense) and myMonsterCardArea[i]:GetPropertyTypeString()=="水")then
			return true;
		end
	end
	for i=0,opponentMonsterCardArea.Length-1 do
		if(opponentMonsterCardArea[i]~=nil and (opponentMonsterCardArea[i]:GetCardGameState()==CS.Assets.Script.Card.CardGameState.FrontAttack or opponentMonsterCardArea[i]:GetCardGameState()==CS.Assets.Script.Card.CardGameState.FrontDefense) and opponentMonsterCardArea[i]:GetPropertyTypeString()=="水")then
			return true;
		end
	end

	return false;
end

function C2370081.LaunchEffect(card)
	local chooseCardEffectProcess = CS.Assets.Script.Duel.EffectProcess.ChooseCardEffectProcess(card,C2370081.ChooseCardJudgeAction,C2370081.ChooseCardCallback, card:GetOwner());
    chooseCardEffectProcess:SetTitle("请选择被装备的怪兽！");
	card:GetDuelCardScript():GetOwner():AddEffectProcess(chooseCardEffectProcess);
end

function C2370081.ChooseCardJudgeAction(launchEffectCard,chooseCard)
	return chooseCard~=nil 
	and (chooseCard :GetCardGameState()==CS.Assets.Script.Card.CardGameState.FrontAttack or chooseCard :GetCardGameState()==CS.Assets.Script.Card.CardGameState.FrontDefense)
	and chooseCard:GetPropertyTypeString()=="水"
end

function C2370081.ChooseCardCallback(launchEffectCard,chooseCard)
	if(not C2370081.ChooseCardJudgeAction(launchEffectCard,chooseCard))then
		return
	end

	launchEffectCard:GetOwner():GetCurrentEffectProcess():AfterFinishProcessFunction();
	
	launchEffectCard:SetEquidMonster(chooseCard);
	chooseCard:AddEquip(launchEffectCard);

	chooseCard:AddCardEffect(launchEffectCard,CS.Assets.Script.Card.CardEffectType.Attack,400);
	chooseCard:AddCardEffect(launchEffectCard,CS.Assets.Script.Card.CardEffectType.Defense,-200);
end

return C2370081