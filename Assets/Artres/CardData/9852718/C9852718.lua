local C9852718={}

function C9852718.InitInfo(card)
	card:SetCardTypeByString("陷阱");
	card:SetName("诀别");
	card:SetTrapTypeByString("通常");
	card:SetEffectInfo("对方战斗阶段从手卡把1张魔法卡送去墓地才能发动。那次战斗阶段结束。场上的表侧表示怪兽直到回合结束时效果无效化。");
end

function C9852718.LaunchEffect(card)
	local changeLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ChangeLifeEffectProcess(500, CS.Assets.Script.Duel.EffectProcess.ChangeLifeType.Effect, card:GetDuelCardScript():GetOwner():GetOpponentPlayer())
    card:GetDuelCardScript():GetOwner():GetOpponentPlayer():AddEffectProcess(changeLifeEffectProcess);
end

return C9852718