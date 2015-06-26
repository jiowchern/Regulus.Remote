Feature: WeaponChancesTable
	In order to 得到特武用
	As a math idiot 擊中公式 
	I want to be 根據亂數表取得對應武器

@mytag
Scenario: 取得0號武器
	Given 武器清單是
	| Id | Rate  |
	| 0  | 0.9   |
	| 2  | 0.033 |
	| 3  | 0.033 |
	| 4  | 0.033 |	
	When 機率是"0.899"	
	#  0 <= Weapon < 0.9
	Then 武器是0

Scenario: 取得2號武器
	Given 武器清單是
	| Id | Rate  |
	| 0  | 0.9   |
	| 2  | 0.033 |
	| 3  | 0.033 |
	| 4  | 0.033 |	
	When 機率是"0.93299" 
	#  0.9 <= Weapon < 0.933
	Then 武器是2

Scenario: 取得3號武器
	Given 武器清單是
	| Id | Rate  |
	| 0  | 0.9   |
	| 2  | 0.033 |
	| 3  | 0.033 |
	| 4  | 0.033 |	
	When 機率是"0.96599999"	
	#  0.933 <= Weapon < 0.966
	Then 武器是3

Scenario: 取得4號武器
	Given 武器清單是
	| Id | Rate  |
	| 0  | 0.9   |
	| 2  | 0.033 |
	| 3  | 0.033 |
	| 4  | 0.033 |	
	When 機率是"0.966"	
	#  0.966 <= Weapon < 1
	Then 武器是4

Scenario: 取得預設武器
	Given 武器清單是
	| Id | Rate  |
	| 0  | 0.9   |
	| 2  | 0.033 |
	| 3  | 0.033 |
	| 4  | 0.033 |	
	When 機率是"1"	
	#  Weapon >= 1
	Then 武器是0

