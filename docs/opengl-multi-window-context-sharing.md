# OpenGL Context Sharing in a Multi-Window Game Engine

When supporting multiple windows in an OpenGL-based game engine, each window normally has its own OpenGL context. However, this does **not** mean that every GPU resource must be duplicated for every window.

If the contexts belong to the same **OpenGL share group**, many GPU resources can be shared between them.

## High-Level Architecture

```text
Window A
  GL Context A
    VAO A
    framebuffer/state A
           │
           ├──────── shared objects ────────┐
           │                                │
Window B                                     │
  GL Context B                              │
    VAO B                                   │
    framebuffer/state B                     │
                                            │
                           VBOs / EBOs
                           Textures
                           Shaders
                           Shader Programs
                           Samplers
```

## Which OpenGL Resources Can Be Shared?

| Resource | Shared between shared GL contexts? |
|---|---|
| Shader | Yes |
| Shader program | Yes |
| VBO / EBO / buffer objects | Yes |
| Texture | Yes |
| Renderbuffer | Yes |
| Sampler | Yes |
| VAO | **No** |
| FBO | **No** |
| GL bindings/state | **No** |

The exact OpenGL rules vary slightly depending on object type and API version, but this is a useful engine-level model.

## Shader Sharing

Shaders and linked shader programs can be shared between contexts as long as the contexts were created as part of the same share group.

This means the engine does not need to compile and link another copy of a shader program for every window.

Instead of:

```text
Window
 └─ Context
     ├─ Meshes
     ├─ Shaders
     ├─ Textures
     └─ Materials
```

a better architecture is:

```text
Engine
│
├─ Shared GL Resources
│   ├─ ShaderPrograms
│   ├─ Buffers
│   ├─ Textures
│   └─ Samplers
│
├─ Window A
│   └─ Context A
│       ├─ VAOs
│       ├─ FBOs
│       └─ Context/render state
│
└─ Window B
    └─ Context B
        ├─ VAOs
        ├─ FBOs
        └─ Context/render state
```

## Vertex Data Does Not Need to Be Duplicated Per Window

A mesh does not necessarily need to be uploaded separately for every window.

Its VBO and EBO can be created once in the shared context group:

```text
Mesh
 ├─ VertexBuffer    <- shared
 ├─ IndexBuffer     <- shared
 └─ CPU metadata
```

Each context can then create its own VAO pointing at the same shared buffers:

```text
Context A
  Mesh VAO -> shared VBO/EBO

Context B
  Mesh VAO -> same shared VBO/EBO
```

This prevents unnecessary duplication of GPU memory.

## Why VAOs Are Different

A VAO mainly describes how vertex-buffer data is interpreted and which buffer bindings are used for drawing.

VAOs are context-local rather than shareable, so a typical design is:

- Keep the actual mesh buffers shared.
- Create a VAO for that mesh in each GL context that needs to render it.

This suggests separating the concepts of:

```text
Mesh GPU Data
    Shared VBO/EBO

Mesh Context State
    VAO per GL context
```

## Context-Local State

Even when GPU objects are shared, OpenGL state itself remains context-local.

For example, each context independently tracks things such as:

- Currently bound shader program
- Currently bound VAO
- Currently bound framebuffer
- Viewport
- Blend state
- Depth state
- Scissor state
- Texture bindings
- Buffer bindings

Therefore, every window renderer should still configure its own render state before drawing.

## Shared Objects and Thread Safety

Sharing a shader program does not mean all operations on it automatically become safe across multiple threads or contexts.

For example:

```csharp
gl.UseProgram(program);
gl.UniformMatrix4(...);
```

If multiple rendering threads operate on shared GL objects at the same time, synchronization may be required.

The engine should distinguish between:

1. **Shared GPU resource ownership**
2. **Context-local rendering state**
3. **Thread synchronization**

## Suggested Engine Structure

A useful engine-level structure would be:

```text
GraphicsDevice / GraphicsShareGroup
│
├─ Shader Manager
├─ Texture Manager
├─ Buffer Manager
├─ Shared Mesh GPU Data
│
└─ Contexts
    │
    ├─ Window A Context
    │   ├─ VAO Cache
    │   ├─ FBOs
    │   └─ Render State
    │
    └─ Window B Context
        ├─ VAO Cache
        ├─ FBOs
        └─ Render State
```

Rather than treating the first window as the owner of shared resources, it can be useful to model an explicit concept such as:

```text
GraphicsShareGroup
```

or:

```text
GLResourceContext
```

The first context may establish the share group internally, but engine code can treat shared graphics resources as belonging to the graphics subsystem rather than to a particular window.

## Example Rendering Flow

```text
Load Mesh
   │
   ├─ Create shared VBO
   └─ Create shared EBO
          │
          ▼
Window A wants to render mesh
   │
   └─ Create/cache VAO for Context A
          │
          ▼
Window B wants to render mesh
   │
   └─ Create/cache VAO for Context B
          │
          ▼
Both contexts use the same VBO/EBO
```

The same principle applies to:

- Shader programs
- Textures
- Uniform buffers
- Shader storage buffers
- Other shareable GPU resources

## Important Context Creation Requirement

Two OpenGL contexts do **not** automatically share resources just because they belong to the same process.

When creating the second and subsequent contexts, they must explicitly be configured to share resources with an existing context.

With Silk.NET and GLFW-backed windowing, this sharing relationship must be established during context/window creation.

Once the contexts belong to the same share group, shared GPU objects can be used across the engine's windows.

## Recommended Mental Model

The most useful mental model is:

```text
GPU Resource
    belongs to
Graphics Share Group

Rendering State
    belongs to
GL Context

GL Context
    belongs to
Window
```

So, in practice:

```text
Shader       -> shared
Texture      -> shared
VBO/EBO      -> shared

VAO          -> per-context
Framebuffer  -> per-context
Render State -> per-context
Viewport     -> per-context
```

This gives a multi-window engine efficient GPU-memory usage while still preserving the isolation required by individual OpenGL contexts.
