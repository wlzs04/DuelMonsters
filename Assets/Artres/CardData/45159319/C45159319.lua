local C45159319={}

function C45159319.InitInfo(card)
	card:SetCardTypeByString("怪兽");
	card:SetName("石像迎击炮");
	card:SetPropertyTypeByString("地");
	card:SetMonsterTypeByString("岩石");

	card:SetLevel(4);
	card:SetOriginalAttackValue(1100);
	card:SetOriginalDefenseValue(2000);
	card:SetEffectInfo("这张卡1个回合1次可以变成里侧守备表示。");
end

return C45159319