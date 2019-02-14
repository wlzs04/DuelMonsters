local C58518520={}

function C58518520.InitInfo(card)
	card:SetCardTypeByString("怪兽");
	card:SetName("净化施效者");
	card:SetPropertyTypeByString("光");
	card:SetMonsterTypeByString("魔法师");

	card:SetLevel(2);
	card:SetAttackValue(0);
	card:SetDefenseValue(900);
	card:SetEffectInfo("①：这张卡作为同调素材送去墓地的场合发动。自己从卡组抽1张。②：这张卡为同调素材的同调怪兽不会被效果破坏。");
end

function C58518520.LaunchEffect(card)
	local reduceLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ReduceLifeEffectProcess(500, CS.Assets.Script.Duel.EffectProcess.ReduceLifeType.Effect, card:GetDuelCardScript():GetOwner():GetOpponentPlayer())
    card:GetDuelCardScript():GetOwner():GetOpponentPlayer():AddEffectProcess(reduceLifeEffectProcess);
end

return C58518520