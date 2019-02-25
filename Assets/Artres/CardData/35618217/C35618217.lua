local C35618217={}

function C35618217.InitInfo(card)
	card:SetCardTypeByString("怪兽");
	card:SetName("月光彩雏");
	card:SetPropertyTypeByString("暗");
	card:SetMonsterTypeByString("兽战士");

	card:SetLevel(4);
	card:SetOriginalAttackValue(1400);
	card:SetOriginalDefenseValue(800);
	card:SetEffectInfo("「月光彩雏」的②的效果1回合只能使用1次。①：1回合1次，从卡组·额外卡组把1只「月光」怪兽送去墓地才能发动。这个回合，把这张卡作为融合素材的场合，可以作为送去墓地的那只怪兽的同名卡来成为融合素材。②：这张卡被效果送去墓地的场合，以自己墓地1张「融合」为对象才能发动。那张卡加入手卡。③：这张卡被除外的场合才能发动。这个回合，对方在战斗阶段中不能把效果发动。");
end

function C35618217.LaunchEffect(card)
	local reduceLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ReduceLifeEffectProcess(500, CS.Assets.Script.Duel.EffectProcess.ReduceLifeType.Effect, card:GetDuelCardScript():GetOwner():GetOpponentPlayer())
    card:GetDuelCardScript():GetOwner():GetOpponentPlayer():AddEffectProcess(reduceLifeEffectProcess);
end

return C35618217