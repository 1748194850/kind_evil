# 🎮 如何设置 PlayerController 参数

## 📍 步骤详解

### 第一步：选择 Player 对象

1. 在 Unity 编辑器的 **Hierarchy** 窗口（通常在左侧）
2. 找到并点击 **Player** 对象
3. Player 对象会被选中（名称高亮显示）

### 第二步：打开 Inspector 窗口

1. 查看 Unity 编辑器右侧的 **Inspector** 窗口
2. 如果看不到 Inspector：
   - 点击菜单栏 `Window` → `General` → `Inspector`
   - 或按快捷键 `Ctrl + 3`（Windows）

### 第三步：找到 PlayerController 组件

在 Inspector 窗口中，向下滚动找到：
```
Player Controller (Script)
```

如果组件标题栏左侧有 ▼ 箭头，说明已展开
如果显示 ▶ 箭头，点击展开组件

### 第四步：设置参数

在 **Player Controller (Script)** 组件中，你会看到：

#### 📦 移动设置
```
移动设置
├── Move Speed: [5]
├── Jump Force: [15]          ← 跳跃力度（建议15-20）
├── Ground Check Distance: [1.0]  ← 点击这里，输入 1.0
└── Ground Layer: [下拉菜单]      ← 点击这里，选择 Everything
```

#### 🦘 跳跃设置
```
跳跃设置
├── Require Grounded: [☐]    ← 取消勾选（允许空中跳跃测试）
└── Allow Air Jump: [✓]       ← 勾选（允许空中跳跃）
```

## 🎯 具体操作

### 操作 1: 设置 Ground Check Distance

1. 找到 **Ground Check Distance** 字段
2. 点击数字框（当前可能是 0.5）
3. 删除旧数字
4. 输入 `1.0`
5. 按 `Enter` 或点击其他地方确认

### 操作 2: 设置 Ground Layer

1. 找到 **Ground Layer** 字段
2. 点击字段右侧的下拉箭头 ▼
3. 会弹出层选择菜单
4. 在菜单中找到并点击 **Everything**
   - 如果列表很长，可以在搜索框输入 "Everything"
   - 或者滚动到列表底部找到

**重要**：如果显示 "Nothing"，必须改为 "Everything"！

### 操作 3: 设置跳跃选项（临时测试）

1. 找到 **跳跃设置** 部分
2. 取消勾选 **Require Grounded**（允许在空中跳跃）
3. 勾选 **Allow Air Jump**（允许空中跳跃）

这样即使没有地面，也可以测试跳跃功能。

## ✅ 设置完成后的检查清单

设置完成后，检查以下内容：

- [ ] **Ground Check Distance** = `1.0`（不是 0.2 或 0.5）
- [ ] **Ground Layer** = `Everything`（不是 `Nothing`）
- [ ] **Require Grounded** = 未勾选（用于测试）
- [ ] **Jump Force** = `15` 或更大
- [ ] **Show Debug Info** = 已勾选（方便调试）

## 🧪 测试

设置完成后：

1. 点击 Unity 编辑器顶部的 **Play** 按钮（▶️）
2. 按 `Space` 键
3. 应该可以跳跃了！

如果 Console 显示 "Player jumped!"，说明跳跃功能正常。

## 🔧 如果还是不行

### 检查清单：

1. **确认组件已添加**
   - Player 对象必须有 PlayerController 组件
   - 如果没有，点击 "Add Component" → 搜索 "PlayerController"

2. **检查输入系统**
   - 打开 `Edit` → `Project Settings` → `Input Manager`
   - 确认有 "Jump" 输入轴，Positive Button 是 "space"

3. **检查 Rigidbody2D**
   - Player 必须有 Rigidbody2D 组件
   - Body Type 应该是 `Dynamic`
   - Gravity Scale 应该是 `1`

4. **查看 Console**
   - 运行场景后查看 Console 窗口
   - 如果有红色错误，先修复错误

## 📸 参数位置示意图

```
Inspector 窗口
┌─────────────────────────────┐
│ Player (Transform)         │
│ ├── Position: (0, 0, 0)     │
│ └── Rotation: (0, 0, 0)     │
│                             │
│ Player Controller (Script) │ ← 找到这个组件
│ ├── 移动设置                │
│ │   ├── Move Speed: 5      │
│ │   ├── Jump Force: 15     │
│ │   ├── Ground Check       │
│ │   │   Distance: [1.0]    │ ← 点击这里输入 1.0
│ │   └── Ground Layer:      │
│ │       [Everything ▼]     │ ← 点击这里选择 Everything
│ ├── 跳跃设置                │
│ │   ├── Require Grounded   │
│ │   │   [☐]                │ ← 取消勾选
│ │   └── Allow Air Jump     │
│ │       [✓]                │ ← 勾选
│ └── 调试                    │
│     └── Show Debug Info    │
│         [✓]                 │
└─────────────────────────────┘
```

## 💡 提示

- **Ground Layer = -1** 在代码中表示 "Everything"（所有层）
- 如果 Inspector 中显示 "Nothing"，说明值是 0，需要改为 "Everything"
- 修改参数后，Unity 会自动保存场景更改
- 如果修改后没有生效，尝试重新运行场景

---

**设置完成后，运行场景按 Space 键测试跳跃！** 🎮

