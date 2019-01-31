local C2370081={}

function C2370081.InitInfo(card)
	card:SetCardTypeByString("魔法");
	card:SetName("钢之甲壳");
	card:SetMagicTypeByString("装备");
	card:SetEffectInfo("只有水属性怪兽才能装备。装备怪兽的攻击力上升400点，守备力下降200点。");
end

function C2370081.CanLaunchEffect(card)
	local myMonsterCardArea = card:GetDuelCardScript():GetOwner():GetMonsterCardArea();
	local opponentMonsterCardArea = card:GetDuelCardScript():GetOwner():GetOpponentPlayer():GetMonsterCardArea();

	for i=0,myMonsterCardArea.Length-1 do
		if(myMonsterCardArea[i]~=nil and myMonsterCardArea[i]:GetPropertyTypeString()=="水")then
			return true;
		end
	end
	for i=0,opponentMonsterCardArea.Length-1 do
		if(opponentMonsterCardArea[i]~=nil and opponentMonsterCardArea[i]:GetPropertyTypeString()=="水")then
			return true;
		end
	end

	return false;
end

function C2370081.LaunchEffect(card)
	
	CS.Assets.Script.GameManager.ShowMessage("请选择被装备的怪兽！")

	local myMonsterCardArea = card:GetDuelCardScript():GetOwner():GetMonsterCardArea();
	local opponentMonsterCardArea = card:GetDuelCardScript():GetOwner():GetOpponentPlayer():GetMonsterCardArea();

	for i=0,myMonsterCardArea.Length-1 do
		if(myMonsterCardArea[i]~=nil and myMonsterCardArea[i]:GetPropertyTypeString()=="水")then
			myMonsterCardArea[i]:GetDuelCardScript():SetClickCallback(card,C2370081.ChooseCardCallback);
		end
	end
	for i=0,opponentMonsterCardArea.Length-1 do
		if(opponentMonsterCardArea[i]~=nil and opponentMonsterCardArea[i]:GetPropertyTypeString()=="水")then
			opponentMonsterCardArea[i]:GetDuelCardScript():SetClickCallback(card,C2370081.ChooseCardCallback);
		end
	end
end

function C2370081.ChooseCardCallback(launchEffectCard,chooseCard)
	if(chooseCard:GetCardType()~=CS.Assets.Script.Card.CardType.Monster or chooseCard:GetPropertyTypeString()~="水")then
		return
	end

	local myMonsterCardArea = chooseCard:GetDuelCardScript():GetOwner():GetMonsterCardArea();
	local opponentMonsterCardArea = chooseCard:GetDuelCardScript():GetOwner():GetOpponentPlayer():GetMonsterCardArea();

	for i=0,myMonsterCardArea.Length-1 do
		if(myMonsterCardArea[i]~=nil and myMonsterCardArea[i]:GetPropertyTypeString()=="水")then
			myMonsterCardArea[i]:GetDuelCardScript():RemoveClickCallback();
		end
	end
	for i=0,opponentMonsterCardArea.Length-1 do
		if(opponentMonsterCardArea[i]~=nil and opponentMonsterCardArea[i]:GetPropertyTypeString()=="水")then
			opponentMonsterCardArea[i]:GetDuelCardScript():RemoveClickCallback();
		end
	end
	chooseCard:AddEquip(launchEffectCard);
	chooseCard:SetAttackNumber(chooseCard:GetAttackNumber()+400);
	chooseCard:SetDefenseNumber(chooseCard:GetDefenseNumber()-200);
	--local addEquipEffectProcess = CS.Assets.Script.Duel.EffectProcess.AddEquipEffectProcess(chooseCard,launchEffectCard, launchEffectCard:GetDuelCardScript():GetOwner());
    --launchEffectCard:GetDuelCardScript():GetOwner():AddEffectProcess(addEquipEffectProcess);
end

return C2370081