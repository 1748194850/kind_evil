# 命名空间冲突问题：Camera 类型使用规范

## 问题

**错误类型：**
- `error CS0118: 'Camera' is a namespace but is used like a type`
- `error CS0234: The type or namespace name 'main' does not exist in the namespace 'Core.Managers.Camera'`

**原因：** 当文件位于包含 `Camera` 关键字的命名空间下（如 `Core.Managers.Camera.Scenes`），或命名空间路径包含 `Camera`（如 `Core.Managers` 下有 `Core.Managers.Camera` 命名空间），编译器可能将 `Camera` 解释为命名空间而不是 `UnityEngine.Camera` 类型。

## 解决方案

**规则：** 在命名空间路径包含 `Camera` 关键字的文件中，必须使用 `UnityEngine.Camera` 完全限定名，不能使用简写 `Camera`。

**适用场景：**
- 命名空间为 `Core.Managers.Camera.*` 或任何包含 `Camera` 的命名空间
- 使用了 `using Core.Managers.Camera.*` 的文件（即使命名空间不包含 Camera）
- 命名空间路径包含 `Camera` 关键字（如 `Core.Managers` 下有 `Core.Managers.Camera` 命名空间）
- 接口定义（如果会被 Camera 命名空间下的类实现）
- 任何可能出现歧义的地方

**修复方法：**
- 将所有 `Camera` 类型替换为 `UnityEngine.Camera`
- 包括字段、方法参数、返回值、静态成员（如 `Camera.main`）等所有使用场景
- 接口定义和实现类保持一致

**注意事项：**
- 同一文件内统一使用完全限定名
- 其他 Unity 类型（如 Transform、GameObject）在类似情况下也应使用完全限定名

## 相关文件

以下文件需使用 `UnityEngine.Camera`：
- `Scenes/0_Core/Managers/Camera/Scenes/BossBattleScene.cs` ✅
- `Scenes/0_Core/Managers/Camera/Scenes/ICameraScene.cs` ✅
- `Scenes/0_Core/Managers/Camera/Scenes/ExplorationScene.cs` ✅
- `Scenes/0_Core/Managers/Camera/Behaviors/ICameraBehavior.cs` ✅
- `Scenes/0_Core/Managers/Camera/Behaviors/FixedBehavior.cs` ✅
- `Scenes/0_Core/Managers/Camera/Behaviors/FollowBehavior.cs` ✅
- `Scenes/0_Core/Managers/Camera/Effects/CameraShakeEffect.cs` ✅
- `Scenes/0_Core/Managers/CameraManager.cs` ✅（使用了 `using Core.Managers.Camera.*`）
