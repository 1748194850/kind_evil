# DontDestroyOnLoad 警告说明

> 最后更新：2026-02-13

## 问题描述

Unity 控制台显示：
```
Some objects were not cleaned up when closing the scene. 
(Did you spawn new GameObjects from OnDestroy?)
The following scene GameObjects were found:
InputManager
```

## 原因说明

这个警告是**正常行为**，不是错误！

### 为什么会显示这个警告？

1. **单例管理器设计**：
   - `InputManager`、`GameManager`、`EventManager` 等管理器使用了 `Singleton` 模式
   - 它们使用 `DontDestroyOnLoad(gameObject)` 来确保在场景切换时不被销毁
   - 这是为了保持游戏状态和功能在场景之间持续存在

2. **Unity 的检测机制**：
   - Unity 在场景关闭时会检查是否有对象没有被清理
   - 使用 `DontDestroyOnLoad` 的对象**故意**不被清理，所以会触发这个警告
   - 这是 Unity 的提醒，不是错误

## 这是正常行为

**你可以安全地忽略这个警告**，因为：

- 单例管理器应该在场景切换时保留
- 这是游戏架构的正常设计
- 不会影响游戏功能
- 不会造成内存泄漏（单例是全局唯一的）

## 重要变更（2026-02-13）

**Singleton 不再自动创建 GameObject**。之前的 `Singleton<T>.Instance` 在找不到实例时会自动 `new GameObject()`，这会丢失所有 Inspector 配置，属于兜底机制，已被删除。

现在的行为：
- `Instance` 找不到实例时 → `LogError` + 返回 `null`
- 必须在场景中**手动添加**管理器对象

这意味着：
- "下次运行场景时会自动重新创建" → **不再适用**
- 如果删除了管理器对象，需要手动重新创建并配置

## 相关脚本

以下管理器使用 `DontDestroyOnLoad`，可能会触发此警告：

| 管理器 | 脚本位置 | 说明 |
|--------|---------|------|
| `InputManager` | `Assets/Scenes/0_Core/Managers/InputManager.cs` | 输入管理 |
| `GameManager` | `Assets/Scenes/0_Core/Managers/GameManager.cs` | 游戏状态管理 |
| `EventManager` | `Assets/Scenes/0_Core/Frameworks/EventSystem/EventManager.cs` | 事件系统 |
| `SceneManager` | `Assets/Scenes/0_Core/Managers/SceneManager.cs` | 场景管理 |
| `TimeManager` | `Assets/Scenes/0_Core/Managers/TimeManager.cs` | 时间管理 |
| `SaveManager` | `Assets/Scenes/0_Core/Frameworks/SaveSystem/SaveManager.cs` | 存档管理 |
| `PoolManager` | `Assets/Scenes/0_Core/Frameworks/Pooling/PoolManager.cs` | 对象池管理 |

## 最佳实践

1. **运行时**：让单例管理器保留，这是正常的设计
2. **编辑器测试**：可以忽略这个警告
3. **发布游戏**：这个警告不会出现在最终游戏中
4. **添加管理器**：必须手动在场景中创建对象并添加脚本组件（Singleton 不会自动创建）

## 如何区分真正的错误？

| 对象类型 | 是否正常 | 处理方式 |
|---------|---------|---------|
| 单例管理器（InputManager 等） | 正常 | 忽略警告 |
| 普通 GameObject 没有被清理 | 异常 | 可能是内存泄漏，需排查 |
| 在 `OnDestroy` 中创建新对象 | 异常 | 需要修复代码 |

## 相关文档

- [常见问题解答](./常见问题解答.md)
- [修复缺失脚本引用](./修复缺失脚本引用.md)
- [代码规范与常见问题](../../开发规范/代码规范与常见问题.md)

---

**维护者**：开发团队
**最后更新**：2026-02-13
