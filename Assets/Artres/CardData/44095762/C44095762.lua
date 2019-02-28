local C44095762={}

function C44095762.InitInfo(card)
	card:SetCardTypeByString("陷阱");
	card:SetName("神圣护罩 -镜之力-");
	card:SetMagicTypeByString("通常");
	card:SetEffectInfo("①：对手怪兽的攻击宣言时可以发动。将对手场上的攻击表示怪兽全部破坏。");
end

function C44095762.CanLaunchEffect(card)
	--在战斗流程并且对方场上存在攻击表示的怪兽时才能发动。
	local currentChainEffectProcess = card:GetDuelScene():GetCurrentChainEffectProcess();
	if(currentChainEffectProcess~=nil and currentChainEffectProcess:GetType()==typeof(CS.Assets.Script.Duel.EffectProcess.AttackEffectProcess))then
		local opponentMonsterCardArea = card:GetOwner():GetOpponentPlayer():GetMonsterCardArea();
		for i=0,opponentMonsterCardArea.Length-1 do
			if(opponentMonsterCardArea[i]~=nil and opponentMonsterCardArea[i]:GetCardGameState()==CS.Assets.Script.Card.CardGameState.FrontAttack)then
				return true;
			end
		end
	end
	return false;
end

function C44095762.LaunchEffect(card)
	local opponentMonsterCardArea = card:GetOwner():GetOpponentPlayer():GetMonsterCardArea();
	for i=0,opponentMonsterCardArea.Length-1 do
		if(opponentMonsterCardArea[i]~=nil and opponentMonsterCardArea[i]:GetCardGameState()==CS.Assets.Script.Card.CardGameState.FrontAttack)then
			local moveCardToTombEffectProcess = CS.Assets.Script.Duel.EffectProcess.MoveCardToTombEffectProcess(opponentMonsterCardArea[i],CS.Assets.Script.Duel.EffectProcess.MoveCardToTombType.Effect, opponentMonsterCardArea[i]:GetOwner());
			opponentMonsterCardArea[i]:GetOwner():AddEffectProcess(moveCardToTombEffectProcess);
		end
	end
end

return C44095762