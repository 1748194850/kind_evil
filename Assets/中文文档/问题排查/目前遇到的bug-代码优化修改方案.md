# 目前遇到的 Bug - 代码优化修改方案

> 本文档基于 `目前遇到的bug.md` 中的问题，记录具体的代码修改方案和实施状态。

---

## 一、红色区域能一直触发连跳 ✅ 已实施

### 问题分析

- **现象**：站在红色伤害区域上可以连续触发跳跃，行为不合理。
- **根因**：`GroundChecker` 的 `groundLayer` 默认 `-1`（Everything），不区分"地面"与"伤害区"。旧代码还有兜底逻辑：`groundLayer == 0` 时静默回退到 Everything。
- **违反规范**：兜底逻辑违反了「禁止兜底机制」代码规范。

### 已实施的修改

| 步骤 | 操作 | 状态 |
|------|------|------|
| 1 | **删除兜底逻辑**：`GroundChecker.cs` 中 `groundLayer == 0` 不再回退到 Everything，改为 `LogError` + 禁用检测 | ✅ |
| 2 | **Everything 警告**：`groundLayer == -1` 时发出 `LogWarning` 提醒配置不当 | ✅ |
| 3 | **场景/层级配置**：在 Unity `Tags and Layers > 图层` 中添加 `Ground` 层 | ⚠️ 需手动操作 |
| 4 | **地面物体**：将所有可站立的地面方块 Layer 设为 `Ground` | ⚠️ 需手动操作 |
| 5 | **伤害区物体**：伤害区域 Layer 保持 `Default` 或设为 `Hazard`，不放在 `Ground` 层 | ⚠️ 需手动操作 |
| 6 | **GroundChecker Inspector**：Ground Layer 字段设为 `Ground`（只勾选 Ground 层） | ⚠️ 需手动操作 |

**涉及文件**：`GroundChecker.cs`（代码已改）；场景中地面与伤害区物体的 Layer 设置（需手动配置）。

---

## 二、遇到侧面墙会粘在墙上下不来 ✅ 已实施

### 问题分析

- **现象**：玩家碰到侧面墙时会被"粘住"，无法下落。
- **根因**：地面检测只检查"是否命中"，未通过碰撞面法线区分顶面和侧面。
- **采用方案**：方案 A（法线角度过滤），这是之前文档中推荐的起步方案。

### 已实施的修改

| 步骤 | 操作 | 状态 |
|------|------|------|
| 1 | **法线过滤**：新增 `maxGroundAngle`（默认 60°），只有 `hit.normal.y >= cos(angle)` 才算地面 | ✅ |
| 2 | **暴露 GroundNormal**：`GroundChecker` 提供 `GroundNormal` 属性，供斜坡移动等后续系统使用 | ✅ |
| 3 | **提取方法**：三处重复的射线循环逻辑提取为 `CheckGroundAtPoint()` 方法 | ✅ |
| 4 | **OnValidate**：编辑器中修改 `maxGroundAngle` 时自动重算阈值 | ✅ |

**涉及文件**：`GroundChecker.cs`

**后续**：若需要攀爬、头顶碰撞等机制，可引入方案 B（Layer 分层）或混合方案。

---

## 三、血量掉到 0 则不能回血 ✅ 已确认 + 补充

### 问题分析

- **现象**：生命值为 0 时无法回血。
- **根因**：`HealthComponent.Heal()` 中 `if (IsDead) return 0f`，死亡状态下不允许回血。
- **结论**：这是**预期行为**，不是 bug。

### 已实施的修改

| 步骤 | 操作 | 状态 |
|------|------|------|
| 1 | **确认无需改 Heal()**：`IsDead` 时 `Heal()` 返回 0 是正确设计 | ✅ |
| 2 | **新增 Revive()**：`HealthComponent.cs` 新增 `Revive(float healthPercentage = 1f)` 方法 | ✅ |
| 3 | **新增 OnRevived 事件**：供 UI、动画等系统监听复活 | ✅ |
| 4 | **复活逻辑**：重置无敌状态 + 闪烁效果，触发 OnRevived → OnHealthChanged → EventSystem | ✅ |

**涉及文件**：`HealthComponent.cs`

---

## 四、额外修复（代码审查中发现）

### 4.1 Singleton 自动创建兜底 ✅ 已修复

- **问题**：`Singleton<T>.Instance` getter 在找不到实例时自动 `new GameObject()`，会丢失所有 Inspector 配置
- **修复**：改为 `UnityEngine.Debug.LogError` + 返回 `null`，不自动创建

### 4.2 PlayerMovement 物理操作在 Update 中 ✅ 已修复

- **问题**：`rb.velocity` 在 `Update()` 中设置，不同帧率下移动速度不一致
- **修复**：`Update()` 只读输入 → `FixedUpdate()` 应用物理

### 4.3 DamageDealer 冷却字典内存泄漏 ✅ 已修复

- **问题**：`Dictionary<GameObject, float>` 用 GameObject 做 key，目标销毁后残留空引用
- **修复**：改为 `Dictionary<int, float>`，使用 `GetInstanceID()` 作 key

---

## 五、实施检查清单

- [x] **红色区域连跳**：GroundChecker 兜底逻辑已删除，需在 Unity 中配置 Ground 层
- [x] **粘墙**：法线角度过滤已实现
- [x] **血量 0 不可回血**：确认为预期行为，已补充 Revive() 方法
- [x] **Singleton 兜底**：已删除自动创建逻辑
- [x] **物理操作**：已移至 FixedUpdate
- [x] **内存泄漏**：已修复字典 key 类型
- [ ] **Unity 手动操作**：在 `Tags and Layers > 图层` 中添加 Ground 层
- [ ] **Unity 手动操作**：地面物体 Layer 设为 Ground，伤害区不放 Ground 层
- [ ] **Unity 手动操作**：GroundChecker Inspector 的 Ground Layer 设为 Ground

---

## 六、注意事项

1. **不添加兜底**：按《代码规范与常见问题》中「禁止兜底机制」要求，配置错误直接报错，不做回退。
2. **先排查配置**：遇到类似问题先检查场景中 Layer、Tag、碰撞体设置，再考虑改代码。
3. **命名空间冲突**：`Core.Utilities` 命名空间下使用 `UnityEngine.Debug` 完全限定名。

---

**维护者**：开发团队
**最后更新**：2026-02-13
**关联文档**：`目前遇到的bug.md`、`代码规范与常见问题.md`
