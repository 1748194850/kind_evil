# Boss战斗场景开发计划

> **项目进度：** 第一个Boss战斗场景开发  
> **创建时间：** 2024年  
> **最后更新：** 2026-02-13  
> **架构：** 依赖注入 + 组件化设计

---

## 📋 目录

1. [阶段零：基础环境搭建](#阶段零基础环境搭建)（Unity 编辑器配置 Checklist）
2. [阶段一：战斗基础系统](#阶段一战斗基础系统)（✅ 代码已完成）
3. [阶段二：Boss核心功能](#阶段二boss核心功能)（移动 + 攻击 + 阶段）
4. [阶段三：战斗交互系统](#阶段三战斗交互系统)（玩家攻击 + **玩家死亡/重生** + 战斗区域 + 状态机）
5. [阶段四：Boss AI和行为](#阶段四boss-ai和行为)
6. [阶段五：视觉效果和反馈](#阶段五视觉效果和反馈)（**玩家 HUD** + Boss 血条 + 特效 + **游戏手感调优**）
7. [阶段六：场景集成和测试](#阶段六场景集成和测试)

---

## ✅ 已完成的系统

### 核心框架
- [x] **事件系统**（EventSystem）- 已完成
- [x] **状态机框架**（StateMachine）- 已完成
- [x] **对象池系统**（Pooling）- 已完成
- [x] **存档系统**（SaveSystem）- 已完成
- [x] **依赖注入系统**（ServiceLocator）- 已完成

### 管理器
- [x] **输入管理器**（InputManager）- 已完成，实现IInputManager接口
- [x] **摄像机管理器**（CameraManager）- 已完成，支持Boss战斗镜头
- [x] **游戏管理器**（GameManager）- 已完成
- [x] **场景管理器**（SceneManager）- 已完成
- [x] **时间管理器**（TimeManager）- 已完成

### 玩家系统
- [x] **玩家组件化系统** - 已完成
  - [x] PlayerMovement - 移动控制（已修复：输入在Update、物理在FixedUpdate）
  - [x] PlayerJump - 跳跃控制
  - [x] GroundChecker - 地面检测（已修复：法线过滤、配置验证、FixedUpdate）
  - [x] PlayerSpriteFlipper - 精灵翻转
- [x] **场景边界系统**（SceneBounds）- 已完成
- [x] **玩家边界检测**（PlayerBoundaryChecker）- 已完成

### 战斗基础框架（2026-02-13 确认已完成）
- [x] **生命值系统** - 已完成
  - [x] IHealth.cs - 生命值接口
  - [x] HealthComponent.cs - 通用生命值组件（含无敌时间、Revive 方法）
  - [x] IDamageable.cs - 可受伤接口
- [x] **伤害系统** - 已完成
  - [x] DamageInfo.cs - 伤害信息数据结构
  - [x] IDamageDealer.cs - 伤害施加接口
  - [x] DamageDealer.cs - 伤害施加组件（已修复：使用 InstanceID 防内存泄漏）
- [x] **Boss 基础框架** - 已完成
  - [x] IBoss.cs - Boss 接口
  - [x] BossController.cs - Boss 主控制器（含 AI 决策循环）
  - [x] BossData.cs - Boss 配置数据（ScriptableObject）
  - [x] TestBossData.asset - 测试用 Boss 数据

### Boss 核心功能（阶段二，2026-02-13 确认已完成）
- [x] **Boss 阶段系统** - 已完成
  - [x] BossPhaseManager.cs - 监听血量变化、自动切换阶段
- [x] **Boss 移动系统** - 已完成
  - [x] BossMovement.cs - 追逐/巡逻/后退/移动到指定位置（已修复 FixedUpdate + 法线过滤）
- [x] **Boss 攻击系统** - 已完成
  - [x] BossAttackBase.cs - 攻击抽象基类（冷却/持续时间/阶段可用性）
  - [x] MeleeAttack.cs - 第一个近战攻击模式
  - [x] IBossAttack.cs - 攻击接口
- [x] **测试辅助** - 已完成
  - [x] BossTestHelper.cs - 按 B 键开始战斗

---

## 阶段零：基础环境搭建

> **目标：** 确保基础场景和玩家系统正常工作，为Boss战斗提供稳定的测试环境  
> **状态：** 代码已全部完成，剩余工作为 **Unity 编辑器配置和验证**  
> **操作指南：** 参考 `教程/阶段零-场景配置指南.md` 逐步操作  
> **预计时间：** 1-2小时

### Unity 编辑器配置 Checklist

**场景与管理器：**
- [ ] 打开或创建测试场景（`test_Scence/test_sence.scene`）
- [ ] 创建 `Managers` 空对象，手动添加所有管理器（⚠️ Singleton 不会自动创建）
  - [ ] GameInitializer（执行顺序设为 -100）
  - [ ] InputManager / CameraManager / EventManager / GameManager
  - [ ] SceneManager / TimeManager / SaveManager / PoolManager
- [ ] 创建并配置 SceneBounds

**地面与环境：**
- [ ] 创建 Ground 对象（Sprite + BoxCollider2D）
- [ ] 在 `Tags and Layers` 中创建 `Ground` 层
- [ ] Ground 对象的 Layer 设为 `Ground`
- [ ] （可选）基础地形、场景背景、玩家出生点

**玩家配置：**
- [ ] Player 添加 Rigidbody2D + BoxCollider2D
- [ ] Player 添加 PlayerMovement / PlayerJump / GroundChecker / PlayerSpriteFlipper / PlayerBoundaryChecker
- [ ] GroundChecker 的 Ground Layer 设为 `Ground`（禁止 Nothing / Everything）
- [ ] PlayerBoundaryChecker 关联 SceneBounds

### 验证 Checklist

- [ ] 运行游戏，Console 无 LogError
- [ ] 玩家可以左右移动
- [ ] 玩家可以跳跃，落地后能再次跳跃
- [ ] 玩家不会粘在墙壁上
- [ ] 玩家不会掉出场景边界
- [ ] 摄像机跟随正常

---

## 阶段一：战斗基础系统

> **目标：** 建立战斗所需的基础系统，使用依赖注入和组件化设计

### 1.1 生命值系统（Health System）✅ 已完成

**优先级：** 🔴 高  
**依赖：** 阶段零完成  
**状态：** ✅ 代码已实现（2026-02-13 确认）

**任务清单：**
- [x] 创建 `IHealth.cs` - 生命值接口
  - [x] 获取当前生命值
  - [x] 获取最大生命值
  - [x] 受伤接口
  - [x] 治疗接口
  - [x] 死亡事件
- [x] 创建 `HealthComponent.cs` - 通用生命值组件
  - [x] 实现IHealth接口
  - [x] 生命值/最大生命值管理
  - [x] 无敌时间（Invincibility）
  - [x] 死亡处理
  - [x] 使用事件系统触发生命值变化事件
  - [x] Revive 复活方法（2026-02-13 新增）
- [x] 创建 `IDamageable.cs` - 可受伤接口
- [ ] 玩家集成生命值系统（需在 Unity 编辑器中操作）
  - [ ] 在Player GameObject上添加HealthComponent
  - [ ] 连接事件系统（触发PlayerHealthChangeEvent）
- [ ] Boss集成生命值系统（需在 Unity 编辑器中操作）
  - [ ] Boss基础类添加HealthComponent
  - [ ] 连接事件系统

**文件路径：**
```
Scenes/0_Core/Frameworks/Combat/
├── IHealth.cs
├── HealthComponent.cs
└── IDamageable.cs (可选)
```

**设计原则：**
- 使用接口（IHealth）实现依赖倒置
- 组件化设计，可复用
- 通过事件系统通知变化

---

### 1.2 伤害系统（Damage System）✅ 已完成

**优先级：** 🔴 高  
**依赖：** 1.1 生命值系统  
**状态：** ✅ 代码已实现（2026-02-13 确认）

**任务清单：**
- [x] 创建 `DamageInfo.cs` - 伤害信息数据结构
  - [x] 伤害值
  - [x] 伤害来源（GameObject引用）
  - [x] 伤害类型（物理/魔法/真实伤害等）
  - [x] 击退力（Vector2）
  - [x] 伤害方向
- [x] 创建 `IDamageDealer.cs` - 伤害施加接口
- [x] 创建 `DamageDealer.cs` - 伤害施加组件
  - [x] 实现IDamageDealer接口
  - [x] 碰撞检测伤害（使用Collider2D）
  - [x] 触发伤害区域
  - [x] 伤害冷却（避免重复伤害，使用 InstanceID 作为 key）
  - [x] 伤害过滤（只伤害特定层/标签）
- [ ] 集成到战斗系统（需在 Unity 编辑器中操作）
  - [ ] 玩家攻击伤害配置
  - [ ] Boss攻击伤害配置

**文件路径：**
```
Scenes/0_Core/Frameworks/Combat/
├── DamageInfo.cs
├── IDamageDealer.cs
└── DamageDealer.cs
```

**设计原则：**
- 伤害信息作为数据结构传递
- 使用接口实现解耦
- 组件化设计

---

### 1.3 Boss基础类框架 ✅ 已完成

**优先级：** 🟡 中  
**依赖：** 1.1 生命值系统  
**状态：** ✅ 代码已实现（2026-02-13 确认）

**任务清单：**
- [x] 创建 `IBoss.cs` - Boss接口
  - [x] 获取Boss状态
  - [x] 获取Boss生命值
  - [x] 开始战斗
  - [x] 结束战斗
- [x] 创建 `BossController.cs` - Boss主控制器
  - [x] 实现IBoss接口
  - [x] Boss基础属性（名称、状态等）
  - [x] 集成HealthComponent
  - [x] 与摄像机系统集成（通过ICameraManager接口）
  - [x] 与事件系统集成（通过IEventManager接口）
  - [x] 使用依赖注入获取服务
- [x] 创建 `BossData.cs` - Boss配置数据（ScriptableObject）
  - [x] Boss基础属性配置
  - [x] 生命值配置
  - [x] 攻击配置
- [ ] Boss预制体基础结构（需在 Unity 编辑器中操作）
  - [ ] 创建Boss GameObject
  - [ ] 添加必要的组件（Rigidbody2D、Collider2D等）
  - [ ] 设置Boss标签（Tag: "Boss"）
  - [ ] 添加BossController组件

**文件路径：**
```
Scenes/1_Gameplay/Boss/
├── IBoss.cs
├── BossController.cs
└── BossData.cs (ScriptableObject)
```

**设计原则：**
- 使用接口（IBoss）实现依赖倒置
- 通过依赖注入获取服务（ICameraManager、IEventManager）
- 配置数据使用ScriptableObject

---

## 阶段二：Boss核心功能

> **目标：** 实现Boss的基本战斗功能

### 2.1 Boss阶段系统 ✅ 已完成

**优先级：** 🟡 中  
**依赖：** 1.3 Boss基础类  
**状态：** ✅ 代码已实现（BossPhaseManager.cs）

**任务清单：**
- [x] 创建 `BossPhaseManager.cs` - Boss阶段管理器
  - [x] 阶段定义（Phase1, Phase2, Phase3 — 由 BossData 阈值决定）
  - [x] 阶段触发条件（生命值百分比）
- [x] 实现Boss阶段切换逻辑
  - [x] 基于生命值百分比的阶段切换
  - [x] 阶段转换事件（OnPhaseChanged + BossPhaseChangeEvent）
  - [x] 不同阶段的属性变化（BossController 中每阶段加速 20%）
- [x] 阶段配置数据（在 BossData 中：phase1Threshold / phase2Threshold）

**阶段设计：**
- **阶段1（100%-70%）：** 基础攻击模式
- **阶段2（70%-40%）：** 增加攻击频率，移动速度+20%
- **阶段3（40%-0%）：** 狂暴模式，移动速度+40%

---

### 2.2 Boss移动系统 ✅ 已完成

**优先级：** 🟡 中  
**依赖：** 1.3 Boss基础类  
**状态：** ✅ 代码已实现（BossMovement.cs，已修复 FixedUpdate + 法线过滤）

**任务清单：**
- [x] 创建 `BossMovement.cs` - Boss移动组件
  - [x] 基础移动（左右移动、跳跃）
  - [x] 跟随玩家移动（Chase 状态）
  - [x] 移动到指定位置（MoveTo 状态 + 回调）
  - [ ] 移动边界限制（使用ISceneBounds接口）— 待后续集成
  - [x] 自动查找 Player（通过 Tag 或手动拖入）
- [x] Boss移动行为
  - [x] 待机时的巡逻（Patrol 状态）
  - [x] 战斗时的追逐策略（Chase 状态 + stoppingDistance）
  - [x] 后退行为（Retreat 状态）
  - [x] 阶段速度缩放（SetSpeedMultiplier）

**文件路径：**
```
Scenes/1_Gameplay/Boss/
└── BossMovement.cs
```

**设计原则：**
- 组件化设计，独立于BossController
- 物理操作在 FixedUpdate，地面检测含法线过滤
- 自动查找 Player（Tag: "Player"），也支持手动拖入

---

### 2.3 Boss攻击系统基础 ✅ 已完成

**优先级：** 🟡 中  
**依赖：** 1.2 伤害系统（与 2.2 移动系统**无依赖**，可并行开发）  
**状态：** ✅ 代码已实现（BossAttackBase.cs + MeleeAttack.cs）

**任务清单：**
- [x] 创建 `IBossAttack.cs` - 攻击接口
  - [x] 开始攻击 / 停止攻击
  - [x] 检查是否可以执行（距离 + 冷却 + 阶段）
- [x] 创建 `BossAttackBase.cs` - 攻击基类
  - [x] 实现IBossAttack接口
  - [x] 攻击冷却时间
  - [x] 攻击持续时间 + 自动结束
  - [x] 阶段可用性检查（IsAvailableInPhase）
- [x] 实现第一个简单攻击模式
  - [x] `MeleeAttack.cs` - 近战攻击（OverlapCircle 检测 + 击退）
  - [ ] 攻击动画/效果 — 待后续实现
  - [x] 伤害区域检测（通过 IDamageable 接口）
- [x] 攻击选择系统（在 BossController.UpdateBattleAI 中实现）
  - [x] 根据阶段选择攻击
  - [x] 随机攻击选择

**文件路径：**
```
Scenes/1_Gameplay/Boss/Attacks/
├── IBossAttack.cs (接口)
├── BossAttackBase.cs (抽象基类)
└── MeleeAttack.cs (第一个攻击)
```

**设计原则：**
- 使用接口（IBossAttack）实现策略模式
- 每个攻击独立实现，易于扩展
- 通过 IDamageable 接口造成伤害

---

## 阶段三：战斗交互系统

> **目标：** 完善玩家与Boss的战斗交互

### 3.1 玩家攻击系统

**优先级：** 🟡 中  
**依赖：** 1.2 伤害系统  
**预计时间：** 3-4小时

**任务清单：**
- [ ] 创建 `PlayerAttack.cs` - 玩家攻击组件
  - [ ] 攻击输入处理（通过IInputManager接口）
  - [ ] 攻击动画
  - [ ] 攻击碰撞检测
  - [ ] 攻击伤害配置
  - [ ] 使用DamageDealer组件
- [ ] 攻击连击系统（可选）
  - [ ] 连击计数
  - [ ] 连击伤害加成
- [ ] 攻击反馈
  - [ ] 击中特效（使用对象池）
  - [ ] 击中音效
  - [ ] 屏幕震动（轻微）

**文件路径：**
```
Scenes/1_Gameplay/Player/Components/
└── PlayerAttack.cs
```

**设计原则：**
- 组件化设计，与PlayerMovement等组件独立
- 使用IInputManager接口获取输入
- 使用对象池管理特效

---

### 3.2 玩家死亡与重生系统

**优先级：** 🟡 中  
**依赖：** 1.1 生命值系统  
**预计时间：** 3-4小时

**任务清单：**
- [ ] 创建 `PlayerDeathHandler.cs` - 玩家死亡处理组件
  - [ ] 监听 HealthComponent 的 OnDeath 事件
  - [ ] 死亡状态处理（禁用输入、播放死亡动画）
  - [ ] 触发 PlayerDeathEvent（通过事件系统）
- [ ] 重生机制
  - [ ] 重生点管理（检查点系统 或 战斗区域入口）
  - [ ] 重生流程（使用 HealthComponent.Revive() 恢复生命值）
  - [ ] 重生后的无敌时间
  - [ ] 重生动画/特效
- [ ] 死亡 UI
  - [ ] 死亡画面（"You Died" / 半透明遮罩）
  - [ ] 重试按钮 / 返回主菜单按钮
  - [ ] 死亡统计（可选：死亡次数、战斗时长）
- [ ] Boss 战斗中的死亡处理
  - [ ] 玩家死亡后 Boss 重置状态（回到战前状态）
  - [ ] 战斗区域解锁（允许重新进入触发）
  - [ ] Boss 生命值是否重置（设计决策）

**文件路径：**
```
Scenes/1_Gameplay/Player/Components/
└── PlayerDeathHandler.cs
Scenes/4_UI/Player/
└── DeathScreenUI.cs
```

**设计原则：**
- 使用 HealthComponent.Revive() 而非 Heal()（死亡状态无法 Heal）
- 通过事件系统解耦死亡通知和 UI 响应
- Boss 战斗中的重生需要与 BossController 协调

---

### 3.3 战斗区域管理

**优先级：** 🟢 低  
**依赖：** 1.3 Boss 基础类  
**预计时间：** 2小时

**任务清单：**
- [ ] 创建 `BossArena.cs` - 战斗区域管理器
  - [ ] 战斗区域边界定义（使用Bounds）
  - [ ] 进入/退出战斗区域检测
  - [ ] 战斗区域锁定（进入后无法离开）
  - [ ] 战斗区域解锁（Boss死亡后）
  - [ ] 与ISceneBounds集成
- [ ] 战斗区域可视化
  - [ ] 使用Gizmos绘制边界（调试用）
- [ ] 与摄像机系统集成
  - [ ] 战斗区域边界传递给Boss镜头配置（通过ICameraManager）

**文件路径：**
```
Scenes/1_Gameplay/Boss/
└── BossArena.cs
```

---

### 3.4 Boss战斗状态机

**优先级：** 🟡 中  
**依赖：** 2.1 Boss阶段系统、2.3 Boss攻击系统  
**预计时间：** 4-5小时

**任务清单：**
- [ ] 使用状态机框架实现Boss行为
  - [ ] **BossIdleState** - 待机状态
  - [ ] **BossChaseState** - 追逐玩家状态
  - [ ] **BossAttackState** - 攻击状态
  - [ ] **BossHurtState** - 受伤状态
  - [ ] **BossDeathState** - 死亡状态
- [ ] 状态转换逻辑
  - [ ] 条件判断（距离、生命值、冷却时间等）
  - [ ] 状态转换事件（使用事件系统）
- [ ] 状态行为实现
  - [ ] 每个状态的具体行为
  - [ ] 状态进入/退出逻辑

**文件路径：**
```
Scenes/1_Gameplay/Boss/States/
├── BossIdleState.cs
├── BossChaseState.cs
├── BossAttackState.cs
├── BossHurtState.cs
└── BossDeathState.cs
```

**设计原则：**
- 使用现有的StateMachine框架
- 每个状态独立实现
- 通过事件系统通知状态变化

---

## 阶段四：Boss AI和行为

> **目标：** 实现智能的Boss行为模式

### 4.1 Boss AI决策系统

**优先级：** 🟡 中  
**依赖：** 3.3 Boss战斗状态机  
**预计时间：** 3-4小时

**任务清单：**
- [ ] AI决策系统
  - [ ] 决策条件（玩家距离、生命值、攻击冷却等）
  - [ ] 行为选择逻辑
  - [ ] 决策优先级
- [ ] 实现基础AI行为
  - [ ] 何时追逐玩家
  - [ ] 何时攻击
  - [ ] 何时后退/防御
- [ ] 不同阶段的AI调整
  - [ ] 阶段1：保守策略
  - [ ] 阶段2：激进策略
  - [ ] 阶段3：狂暴策略

**设计原则：**
- 决策逻辑与状态机结合
- 可配置的AI参数（在BossData中）

---

### 4.2 多种攻击模式

**优先级：** 🟡 中  
**依赖：** 2.3 Boss攻击系统基础  
**预计时间：** 6-8小时

**任务清单：**
- [ ] **近战攻击**
  - [ ] MeleeAttack（已实现）
  - [ ] ChargeAttack - 冲刺攻击
  - [ ] JumpAttack - 跳跃攻击
- [ ] **远程攻击**（如果Boss需要）
  - [ ] ProjectileAttack - 投射物攻击
  - [ ] 弹道系统（使用对象池管理投射物）
- [ ] **范围攻击**
  - [ ] AreaAttack - 圆形范围攻击
  - [ ] GroundShockAttack - 地面冲击波
- [ ] **特殊攻击**（阶段2+）
  - [ ] ComboAttack - 连击攻击
  - [ ] ChargeUpAttack - 蓄力攻击

**文件路径：**
```
Scenes/1_Gameplay/Boss/Attacks/
├── MeleeAttack.cs
├── ChargeAttack.cs
├── JumpAttack.cs
├── ProjectileAttack.cs
├── AreaAttack.cs
└── GroundShockAttack.cs
```

**设计原则：**
- 所有攻击实现IBossAttack接口
- 使用对象池管理投射物和特效
- 每个攻击独立配置

---

### 4.3 Boss技能系统（可选）

**优先级：** 🟢 低  
**依赖：** 4.2 多种攻击模式  
**预计时间：** 4-5小时

**任务清单：**
- [ ] 技能系统框架
  - [ ] 技能冷却管理
  - [ ] 技能资源消耗（如果有）
- [ ] 实现1-2个特殊技能
  - [ ] 技能1：例如"召唤小怪"或"护盾"
  - [ ] 技能2：例如"范围爆炸"或"治疗"
- [ ] 技能触发条件
  - [ ] 生命值阈值触发
  - [ ] 时间间隔触发

---

## 阶段五：视觉效果和反馈

> **目标：** 增强战斗的视觉体验和反馈

### 5.1 玩家 HUD（血条 + 状态）

**优先级：** 🟡 中  
**依赖：** 1.1 生命值系统  
**预计时间：** 2-3小时

**任务清单：**
- [ ] 创建玩家血条 UI
  - [ ] 血条预制体（Screen Space - Overlay Canvas）
  - [ ] 血条动画（受伤时平滑减少、治疗时平滑增加）
  - [ ] 低血量警告效果（血条变红 / 屏幕边缘泛红）
- [ ] 血条数据绑定
  - [ ] 监听 HealthComponent.OnHealthChanged 事件
  - [ ] 显示当前/最大生命值数字（可选）
- [ ] UI 布局
  - [ ] 屏幕左上角固定位置
  - [ ] 适配不同分辨率

**文件路径：**
```
Scenes/4_UI/Player/
└── PlayerHealthBar.cs
```

**设计原则：**
- 通过事件系统监听，UI 层与游戏逻辑完全解耦
- 复用 HealthComponent 的 OnHealthChanged 事件，不需要额外轮询

---

### 5.2 Boss 血条 UI

**优先级：** 🟡 中  
**依赖：** 1.1 生命值系统、2.1 Boss 阶段系统  
**预计时间：** 3-4小时

**任务清单：**
- [ ] 创建Boss血条UI
  - [ ] 血条预制体（UI Canvas）
  - [ ] 血条动画（受伤时减少）
  - [ ] 阶段指示器（显示当前阶段）
- [ ] 血条显示/隐藏逻辑
  - [ ] 进入战斗时显示（通过事件系统）
  - [ ] 战斗结束时隐藏
- [ ] 血条位置
  - [ ] 屏幕顶部固定位置
  - [ ] 或跟随Boss（世界空间UI）

**文件路径：**
```
Scenes/4_UI/Boss/
└── BossHealthBar.cs
```

**设计原则：**
- 通过事件系统监听Boss战斗开始/结束
- UI组件独立，可复用

---

### 5.3 战斗特效

**优先级：** 🟢 低  
**依赖：** 3.1 玩家攻击系统、2.3 Boss攻击系统  
**预计时间：** 4-6小时

**任务清单：**
- [ ] 攻击特效
  - [ ] 玩家攻击命中特效（使用对象池）
  - [ ] Boss攻击特效（使用对象池）
  - [ ] 攻击轨迹特效
- [ ] 受伤特效
  - [ ] Boss受伤闪烁（使用SpriteRenderer）
  - [ ] 受伤粒子效果（使用对象池）
  - [ ] 伤害数字显示（可选，使用对象池）
- [ ] 阶段转换特效
  - [ ] 阶段转换动画
  - [ ] 阶段转换特效
  - [ ] 阶段转换音效

**设计原则：**
- 所有特效使用对象池管理
- 特效组件化，可配置

---

### 5.4 镜头效果

**优先级：** 🟢 低  
**依赖：** 摄像机系统（已完成）  
**预计时间：** 2-3小时

**任务清单：**
- [ ] 镜头抖动集成
  - [ ] Boss攻击时镜头抖动（通过ICameraManager）
  - [ ] 玩家受到伤害时镜头抖动
  - [ ] Boss死亡时镜头效果
- [ ] 镜头缩放效果（可选）
  - [ ] 特殊攻击时的镜头缩放
- [ ] 慢动作效果（可选）
  - [ ] 关键时刻的慢动作（使用TimeManager）

**设计原则：**
- 通过ICameraManager接口调用镜头效果
- 使用TimeManager控制时间缩放

---

### 5.5 音效和音乐

**优先级：** 🟢 低  
**依赖：** 无  
**预计时间：** 2-3小时

**任务清单：**
- [ ] Boss战斗音乐
  - [ ] 战斗开始音乐
  - [ ] 不同阶段的音乐变化
  - [ ] 战斗结束音乐
- [ ] Boss音效
  - [ ] 攻击音效
  - [ ] 受伤音效
  - [ ] 死亡音效
  - [ ] 阶段转换音效
- [ ] 玩家音效
  - [ ] 攻击命中音效
  - [ ] 受到伤害音效

**注意：** 如果还没有音频管理器，需要先创建AudioManager

---

### 5.6 游戏手感调优（Game Feel）

**优先级：** 🟡 中  
**依赖：** 3.1 玩家攻击系统、2.3 Boss攻击系统  
**预计时间：** 3-4小时（持续迭代）

**任务清单：**
- [ ] 击中停顿（Hitstop/Freeze Frame）
  - [ ] 玩家攻击命中时短暂冻结（2-4帧，使用 TimeManager）
  - [ ] Boss 重击命中时短暂冻结
  - [ ] 可配置的冻结时长（在 DamageDealer 或攻击数据中）
- [ ] 攻击判定调优
  - [ ] 攻击判定帧窗口（Attack Active Frames）
  - [ ] 攻击前摇/后摇时长
  - [ ] 判定区域大小与动画匹配
- [ ] 受击反馈
  - [ ] 受击硬直时长（Hitstun Duration）
  - [ ] 击退力度曲线（不是线性，而是先快后慢）
  - [ ] 击退方向计算（基于攻击者位置）
- [ ] 移动手感
  - [ ] 加速/减速曲线（避免移动过于生硬）
  - [ ] 跳跃手感（土狼时间 Coyote Time / 跳跃缓冲 Jump Buffer）
  - [ ] 空中控制力度

**设计原则：**
- 所有手感参数通过 ScriptableObject 或 Inspector 配置，方便快速迭代
- 使用 TimeManager.TimeScale 实现击中停顿
- 手感调优是**持续过程**，不是一次性完成

---

## 阶段六：场景集成和测试

> **目标：** 将Boss战斗整合到游戏场景中并测试

### 6.1 Boss战斗场景搭建

**优先级：** 🟡 中  
**依赖：** 阶段零完成、3.2 战斗区域管理  
**预计时间：** 2-3小时

**任务清单：**
- [ ] 在基础场景上添加Boss战斗区域
  - [ ] 战斗区域布局设计
  - [ ] 战斗区域边界设置（使用BossArena）
  - [ ] Boss战斗区域装饰
- [ ] Boss场景元素
  - [ ] Boss战斗平台（如果有特殊地形）
  - [ ] 障碍物（如果有）
  - [ ] Boss区域背景元素
- [ ] Boss场景边界
  - [ ] 战斗区域边界（使用SceneBounds系统）
  - [ ] 摄像机边界（与Boss战斗镜头集成，通过ICameraManager）

---

### 6.2 Boss触发系统

**优先级：** 🟡 中  
**依赖：** 1.3 Boss基础类  
**预计时间：** 2-3小时

**任务清单：**
- [ ] 创建 `BossTrigger.cs` - Boss触发组件
  - [ ] 进入区域触发Boss战斗（使用Collider2D）
  - [ ] 触发后锁定战斗区域
- [ ] Boss战斗开始流程
  - [ ] 过场动画（可选）
  - [ ] 切换摄像机到Boss战斗镜头（通过ICameraManager）
  - [ ] 显示Boss血条（通过事件系统）
  - [ ] 播放战斗音乐
- [ ] Boss战斗结束流程
  - [ ] Boss死亡动画
  - [ ] 切换摄像机回跟随镜头（通过ICameraManager）
  - [ ] 隐藏Boss血条（通过事件系统）
  - [ ] 解锁战斗区域
  - [ ] 保存Boss击败状态（通过存档系统）

**文件路径：**
```
Scenes/1_Gameplay/Boss/
└── BossTrigger.cs
```

**设计原则：**
- 通过事件系统触发战斗开始/结束
- 使用接口获取服务（ICameraManager）

---

### 6.3 存档系统集成

**优先级：** 🟢 低  
**依赖：** 存档系统（已完成）  
**预计时间：** 1-2小时

**任务清单：**
- [ ] Boss击败状态保存
  - [ ] Boss死亡时调用存档系统
  - [ ] 检查Boss是否已击败（避免重复战斗）
- [ ] Boss重生逻辑（如果需要）
  - [ ] 已击败的Boss不再出现
  - [ ] 或允许重复挑战

---

### 6.4 功能测试

**优先级：** 🟡 中  
**依赖：** 所有阶段  
**预计时间：** 4-6小时

**任务清单：**
- [ ] 基础功能测试
  - [ ] Boss生命值系统
  - [ ] 伤害系统
  - [ ] 攻击系统
  - [ ] 阶段切换
- [ ] 边界情况测试
  - [ ] Boss在边界时的行为
  - [ ] 玩家在边界时的行为
  - [ ] 极端生命值情况
- [ ] 性能测试
  - [ ] 帧率检查
  - [ ] 内存使用检查
  - [ ] 对象池使用检查

---

### 6.5 平衡性调整

**优先级：** 🟡 中  
**依赖：** 6.4 功能测试  
**预计时间：** 持续调整

**任务清单：**
- [ ] 难度平衡
  - [ ] Boss生命值调整
  - [ ] 攻击伤害调整
  - [ ] 攻击频率调整
  - [ ] 玩家伤害调整
- [ ] 游戏节奏
  - [ ] 战斗时长
  - [ ] 阶段转换时机
  - [ ] 攻击模式频率

---

## 📊 开发优先级总结

### ✅ 已完成（阶段一 + 阶段二）
1. ~~**生命值系统**~~ - ✅ 代码已实现
2. ~~**伤害系统**~~ - ✅ 代码已实现
3. ~~**Boss基础类**~~ - ✅ 代码已实现
4. ~~**Boss阶段系统**~~ - ✅ BossPhaseManager 已实现
5. ~~**Boss移动系统**~~ - ✅ BossMovement 已实现（含 FixedUpdate 修复）
6. ~~**Boss攻击系统**~~ - ✅ BossAttackBase + MeleeAttack 已实现

### 🔴 高优先级（当前最需要做的）
7. **场景配置完善** - 在 Unity 编辑器中配置所有组件和层
8. **玩家系统验证** - 在 Unity 中实际测试完整战斗流程
9. **玩家攻击系统** - 让玩家可以攻击 Boss（代码开发）

### 🟡 中优先级（核心功能）
10. **玩家死亡/重生系统** - 完整的战斗循环
11. **Boss战斗状态机** - 更智能的 AI 行为（替代当前简单 AI）
12. **Boss AI决策** - 智能战斗
13. **多种攻击模式** - 战斗多样性
14. **玩家血条 UI** - 玩家状态反馈
15. **Boss血条UI** - Boss 状态反馈
16. **游戏手感调优** - 击中停顿、受击反馈、移动手感
17. **Boss战斗场景搭建** - 完整体验
18. **Boss触发系统** - 场景集成

### 🟢 低优先级（增强体验）
19. **战斗特效** - 视觉增强
20. **镜头效果** - 体验增强
21. **音效音乐** - 氛围营造
22. **Boss技能系统** - 高级功能
23. **存档集成** - 进度保存

---

## 🎯 第一个Boss的最小可行版本（MVP）

如果你想快速看到效果，可以先完成以下核心功能：

### MVP Checklist
- [x] 摄像机系统（已完成）
- [x] 场景边界系统（已完成）
- [x] 玩家组件系统（已完成，含 FixedUpdate 修复）
- [x] 事件系统（已完成）
- [x] 状态机框架（已完成）
- [x] 依赖注入系统（已完成）
- [x] 生命值系统（已完成，含 Revive 方法）
- [x] 伤害系统（已完成，含 InstanceID 修复）
- [x] Boss基础类（已完成，IBoss + BossController + BossData）
- [x] Boss 阶段系统（已完成，BossPhaseManager）
- [x] Boss 移动系统（已完成，BossMovement — 追逐/巡逻/后退）
- [x] 一个简单攻击模式（已完成，MeleeAttack — 近战挥击 + 击退）
- [ ] **场景配置完善**（最优先 — 需在 Unity 编辑器中操作）
- [ ] **玩家系统验证**（需在 Unity 编辑器中测试完整战斗流程）
- [ ] 玩家攻击系统
- [ ] 玩家死亡/重生流程（使用 Revive 方法）
- [ ] 玩家血条 UI
- [ ] Boss 血条 UI
- [ ] Boss 触发系统
- [ ] Boss 战斗场景搭建

**预计时间：** 18-25小时（包含场景搭建、系统验证和 UI）

---

## 📝 开发建议

1. **先验证现有系统**：确保场景边界、玩家组件、依赖注入都正常工作
2. **按阶段开发**：严格按照阶段顺序，完成一个阶段再进入下一个
3. **先做MVP**：先实现最小可行版本，看到效果后再完善
4. **频繁测试**：每完成一个功能就测试，及时发现问题
5. **保持简单**：第一个Boss不要设计得太复杂，先验证系统
6. **使用接口**：所有系统都通过接口交互，符合依赖倒置原则
7. **组件化设计**：每个功能独立组件，易于测试和维护
8. **使用事件系统**：系统间通信使用事件系统，降低耦合

---

## 🔄 更新日志

- **2024年** - 初始创建TODO文档
- **2024年** - 重新规划，基于当前架构（依赖注入+组件化）
- **2024年** - 移除七层渲染系统相关引用
- **2024年** - 明确已完成的系统，调整开发顺序
- **2026-02-13** - 同步代码实际状态：标记阶段一（生命值/伤害/Boss基础）为已完成；更新 MVP Checklist；记录代码修复（GroundChecker、Singleton、PlayerMovement、DamageDealer）
- **2026-02-13** - 计划完善：精简阶段零为 Checklist；新增玩家死亡/重生系统（3.2）；新增玩家 HUD（5.1）；新增游戏手感调优（5.6）；修复 2.3 攻击系统依赖关系；更新优先级总结和 MVP Checklist
- **2026-02-13** - 同步阶段二完成状态：标记 BossPhaseManager / BossMovement / BossAttackBase / MeleeAttack 为已完成；修复 BossMovement 地面检测（FixedUpdate + 法线过滤 + Ground Layer 验证）；修复 MeleeAttack targetLayer 默认值；修复 BossTestHelper 命名空间

---

## 🏗️ 架构说明

### 设计原则
- **依赖倒置原则**：所有系统通过接口交互
- **单一职责原则**：每个组件只负责一个功能
- **组件化设计**：功能拆分为独立组件
- **事件驱动**：系统间通信使用事件系统

### 依赖注入流程
1. GameInitializer在Awake中注册所有服务（执行顺序：-100）
2. 组件通过ServiceLocator获取接口
3. 如果服务未注册，直接报错（不向后兼容）
4. **Singleton 不会自动创建** — 必须手动在场景中添加管理器 GameObject

### 事件系统使用
- Boss战斗开始：`BossBattleStartedEvent`
- Boss阶段转换：`BossPhaseChangeEvent`
- Boss死亡：`BossDeathEvent`
- 玩家生命值变化：`PlayerHealthChangeEvent`
- 玩家死亡：`PlayerDeathEvent`（新增）
- 玩家重生：`PlayerReviveEvent`（新增）

---

**下一步行动：** 阶段一和阶段二的代码已全部完成，当前最高优先级是：
1. **在 Unity 编辑器中完成场景配置**（参考 `教程/阶段零-场景配置指南.md`）
2. **在 Unity 编辑器中配置战斗组件**（参考 `教程/阶段一-战斗基础系统配置指南.md` + `教程/阶段二-Boss核心功能配置指南.md`）
3. **在 Unity 中测试完整战斗流程**（按 B 键开始战斗，验证追逐/攻击/阶段切换）
4. **开始阶段三：玩家攻击系统和死亡/重生流程的代码开发**
