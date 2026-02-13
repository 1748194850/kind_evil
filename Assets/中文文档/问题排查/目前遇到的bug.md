# 目前遇到的 Bug

> 记录开发过程中遇到的问题、分析和解决状态。

---

## Bug 1：红色区域能一直触发连跳 ✅ 已解决

**现象**：站在红色伤害区域上可以连续触发跳跃，行为不合理。

**原因分析**：`GroundChecker` 的 `groundLayer` 默认为 `-1`（Everything），红色伤害区域也被当作地面，`IsGrounded` 为 true，允许跳跃。

**解决方案**（2026-02-13 已实施）：
1. `GroundChecker.cs` 删除了 `groundLayer == Nothing` 时回退到 Everything 的兜底逻辑
2. `groundLayer` 默认值改为 `0`（Nothing），必须在 Inspector 中手动配置为 `Ground` 层
3. 配置错误时直接 `LogError` 并禁用检测，不做任何兜底
4. **Unity 操作**：在 `Tags and Layers > 图层` 中添加 `Ground` 层，地面物体设为 Ground 层，伤害区域不放在 Ground 层

---

## Bug 2：遇到侧面的墙就会粘在墙上下不来 ✅ 已解决

**现象**：玩家碰到侧面墙时会被"粘住"，无法下落。

**原因分析**：地面检测只做向下射线，未区分顶面（可站立）与侧面（墙）。射线打到墙的表面时被误判为地面。

**解决方案**（2026-02-13 已实施）：
1. `GroundChecker.cs` 新增法线角度过滤（`maxGroundAngle = 60°`）
2. 只有 `hit.normal.y >= cos(maxGroundAngle)` 的表面才算地面
3. 垂直墙面的 `normal.y ≈ 0`，远小于阈值，会被正确排除
4. 暴露 `GroundNormal` 属性，供后续斜坡移动等系统使用

**后续**：若需要攀爬机制，可在此基础上通过法线角度判断侧面，或引入 Layer 分层方案。

---

## Bug 3：当血量掉到 0 则不能回血 ✅ 已确认

**现象**：生命值为 0 时无法回血。

**结论**：这是**预期行为**，不是 bug。`HealthComponent.Heal()` 在 `IsDead` 状态下返回 0，这是正确的设计。

**补充**（2026-02-13 已实施）：
- `HealthComponent.cs` 新增 `Revive(float healthPercentage)` 方法，专门用于复活
- 新增 `OnRevived` 事件，让 UI/动画等系统可以监听复活
- 明确区分：`Heal()` = 治疗（活着时用），`Revive()` = 复活（死亡时用）

---

## 更新记录

- 2026-02-13：三个 bug 全部解决/确认，代码已修改，详见 `目前遇到的bug-代码优化修改方案.md`
