local C50277973={}

C50277973.propertyTypeIndex=0;

function C50277973.InitInfo(card)
	card:SetCardTypeByString("陷阱");
	card:SetName("镜像沼泽人");
	card:SetMagicTypeByString("永续");
	card:SetEffectInfo("①：可以宣言种族和属性各1个发动。这张卡发动后变为拥有宣言的种族和属性的通常怪兽（4星·攻1800/守1000）特殊召唤到怪兽区。这张卡也当作陷阱卡使用。");
end

function C50277973.CanLaunchEffect(card)
	return card:GetOwner():GetMonsterCardNumberInArea()<5;
end

function C50277973.LaunchEffect(card)
	local selectItemEffectProcess = CS.Assets.Script.Duel.EffectProcess.SelectItemEffectProcess(card,typeof(CS.Assets.Script.Card.PropertyType),C50277973.SelectPropertyTypeCallback, card:GetOwner());
    card:GetOwner():AddEffectProcess(selectItemEffectProcess);
end

function C50277973.SelectPropertyTypeCallback(card,index)
	C50277973.propertyTypeIndex=index;

	local selectItemEffectProcess = CS.Assets.Script.Duel.EffectProcess.SelectItemEffectProcess(card,typeof(CS.Assets.Script.Card.MonsterType),C50277973.SelectMonsterTypeCallback, card:GetOwner());
    card:GetOwner():AddEffectProcess(selectItemEffectProcess);
end

function C50277973.SelectMonsterTypeCallback(card,index)
	card:SetCardTypeByString("怪兽");
	card:SetPropertyType(C50277973.propertyTypeIndex);
	card:SetMonsterType(index);
	card:SetLevel(4);
	card:SetOriginalAttackValue(1800);
	card:SetOriginalDefenseValue(1000);
	
	local callMonsterEffectProcess = CS.Assets.Script.Duel.EffectProcess.CallMonsterEffectProcess(card,CS.Assets.Script.Card.CardGameState.Unknown, card:GetDuelCardScript():GetOwner());
    card:GetDuelCardScript():GetOwner():AddEffectProcess(callMonsterEffectProcess);
end

return C50277973