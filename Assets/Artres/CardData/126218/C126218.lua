local C126218={}

function C126218.InitInfo(card)
	card:SetCardTypeByString("陷阱");
	card:SetName("悪魔的骰子");
	card:SetMagicTypeByString("通常");
	card:SetEffectInfo("①：掷1次骰子。对手场上的怪兽的攻击力、守备力直到回合结束时下降掷出的数×100。");
end

function C126218.CanLaunchEffect(card)
	return card:GetOwner():GetOpponentPlayer():GetMonsterCardNumberInArea()>0;
end

function C126218.LaunchEffect(card)
	local throwDiceEffectProcess = CS.Assets.Script.Duel.EffectProcess.ThrowDiceEffectProcess(card,C126218.ThrowDiceCallBack,card:GetOwner());
    card:GetOwner():AddEffectProcess(throwDiceEffectProcess);
	
end

function C126218.ThrowDiceCallBack(card,resultNumber)
	local opponentMonsterArea = card:GetOwner():GetOpponentPlayer():GetMonsterCardArea();
	for i=0,opponentMonsterArea.Length-1 do
		if(opponentMonsterArea[i]~=nil)then
			local cardEffect = opponentMonsterArea[i]:AddCardEffect(card,CS.Assets.Script.Card.CardEffectType.Attack,-(100*resultNumber));
			cardEffect:SetEffectLimit(0,CS.Assets.Script.Duel.Rule.DuelProcess.End);
		end
	end
end

return C126218