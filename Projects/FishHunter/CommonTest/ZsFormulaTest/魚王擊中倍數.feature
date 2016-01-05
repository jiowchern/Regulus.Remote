Feature: 魚王擊中倍數
	In order to 計算魚王擊中倍數
	As a math 算法
	I want to 回傳計算倍數 

@mytag
Scenario: 魚王擊中倍數
	Given 擊中魚的資料是
	| FishId | FishOdds | FishStatus | FishType      | GraveGoods |
	| 1      | 100      | KING       | TROPICAL_FISH |            |

	When 祭品是
	| FishId | FishOdds | FishStatus | FishType      | GraveGoods |
	| 1      | 1        | NORMAL     | TROPICAL_FISH |            |
	| 1      | 1        | NORMAL     | TROPICAL_FISH |            |

	Then 加總倍數是 102
