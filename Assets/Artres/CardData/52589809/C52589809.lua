local C52589809={}

function C52589809.InitInfo(card)
	card:SetCardTypeByString("怪兽");
	card:SetName("风来王 野风");
	card:SetPropertyTypeByString("暗");
	card:SetMonsterTypeByString("恶魔");

	card:SetLevel(4);
	card:SetAttackValue(1700);
	card:SetDefenseValue(1300);
	card:SetEffectInfo("①：自己场上有攻击力1500以下的恶魔族调整存在的场合，可以将这张卡从手牌特殊召唤。用这个方法特殊召唤过的回合，自己从额外卡组特殊召唤的只能是同调怪兽。②：可以将墓地的这张卡除外发动。从卡组将1只攻击力1500以下的恶魔族调整加入手牌。这个效果在这张卡送入墓地的回合不能发动。");
end

function C52589809.LaunchEffect(card)
	local reduceLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ReduceLifeEffectProcess(500, CS.Assets.Script.Duel.EffectProcess.ReduceLifeType.Effect, card:GetDuelCardScript():GetOwner():GetOpponentPlayer())
    card:GetDuelCardScript():GetOwner():GetOpponentPlayer():AddEffectProcess(reduceLifeEffectProcess);
end

return C52589809