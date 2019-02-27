local C14315573={}

function C14315573.InitInfo(card)
	card:SetCardTypeByString("陷阱");
	card:SetName("攻击无力化");
	card:SetMagicTypeByString("反击");
	card:SetEffectInfo("①：对手怪兽的攻击宣言时，可以以那1只攻击怪兽为对象发动。使那次攻击无效。那之后，结束战斗阶段。");
end

function C14315573.CanLaunchEffect(card)
	local currentChainEffectProcess = card:GetDuelScene():GetCurrentChainEffectProcess();
	if(currentChainEffectProcess~=nil and currentChainEffectProcess:GetType()==typeof(CS.Assets.Script.Duel.EffectProcess.AttackEffectProcess))then
		return true;
	end
	return false;
end

function C14315573.LaunchEffect(card)
	--攻击无效并结束战斗流程。
	if(not C14315573.CanLaunchEffect(card))then
		return;
	end
	
	local currentChainEffectProcess = card:GetDuelScene():GetCurrentChainEffectProcess();
	currentChainEffectProcess:Stop();

	card:GetDuelScene():StopBattlePhaseType();

end

return C14315573