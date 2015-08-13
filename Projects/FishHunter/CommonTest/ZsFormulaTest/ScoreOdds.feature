Feature: ScoreOdds
	In order to : 分數回饋
	As a math : 擊中公式
	I want to be : 根據亂數表取得相對分數

Scenario: 魚的賠率為1
	Given 賠率表
	| Id | Rate  |
	| 1  | 0.9   |
	| 2  | 0.025 |
	| 3  | 0.025 |
	| 5  | 0.025 |	
	| 10  | 0.025 |		
	When 機率是0
	#  0 <= Weapon < 0.9
	Then 賠率為1


Scenario: 魚的賠率為2
	Given 賠率表
	| Id | Rate  |
	| 1  | 0.9   |
	| 2  | 0.025 |
	| 3  | 0.025 |
	| 5  | 0.025 |	
	| 10  | 0.025 |		
	When 機率是0.9
	#  0.9 <= Weapon < 0.925
	Then 賠率為2

Scenario: 魚的賠率為3
	Given 賠率表
	| Id | Rate  |
	| 1  | 0.9   |
	| 2  | 0.025 |
	| 3  | 0.025 |
	| 5  | 0.025 |	
	| 10  | 0.025 |		
	When 機率是0.925
	#  0.925 <= Weapon < 0.950
	Then 賠率為3

Scenario: 魚的賠率為5
	Given 賠率表
	| Id | Rate  |
	| 1  | 0.9   |
	| 2  | 0.025 |
	| 3  | 0.025 |
	| 5  | 0.025 |	
	| 10  | 0.025 |		
	When 機率是0.950
	#  0.950 <= Weapon < 0.975
	Then 賠率為5

Scenario: 魚的賠率為10
	Given 賠率表
	| Id | Rate  |
	| 1  | 0.9   |
	| 2  | 0.025 |
	| 3  | 0.025 |
	| 5  | 0.025 |	
	| 10  | 0.025 |		
	When 機率是0.975
	#  0.975 <= Weapon < 1.0
	Then 賠率為10
