﻿Feature: 大章魚武器規則
	In order to 避免武器判定錯誤
	As 算法伺服器
	I want to 回傳死亡魚

@mytag
Scenario: 大章魚武器規則
	Given 觸發武器類型為
	| BulletId | WeaponType       | WeaponBet | WeaponOdds | TotalHits |
	| 1        | BIG_OCTOPUS_BOMB | 1000      | 1          | 1         |
		
	And 擊中魚清單為
	| FishId | FishOdds | FishStatus | FishType      | GraveGoods |
	| 1      | 1        | NORMAL     | TROPICAL_FISH |            |
	| 2      | 1        | NORMAL     | SPECIAL_THUNDER_BOMB |            |

	When 過瀘武器無效對象
	
	Then 擊殺的魚是
	
