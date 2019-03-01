local C45121025={}

function C45121025.InitInfo(card)
	card:SetCardTypeByString("怪兽");
	card:SetName("黑影鬼王");
	card:SetPropertyTypeByString("地");
	card:SetMonsterTypeByString("兽战士");

	card:SetLevel(4);
	card:SetOriginalAttackValue(1200);
	card:SetOriginalDefenseValue(1400);
	card:SetEffectInfo("被黑影附体的恶鬼。用惊人的速度突击。");
end

return C45121025