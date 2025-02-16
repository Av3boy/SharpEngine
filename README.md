# SharpEngine (Name already taken, will be renamed soon)
SharpEngine is a game engine written in purely in C# using [OpenTK.net](https://opentk.net) as a wrapper for the graphics rendering pipeline.

NOTE: We are considering of refactoring the engine to use [Silk.NET](https://github.com/dotnet/Silk.NET)

## Repository contents
The [Game Engine Architecture by Jason Gregory](https://www.gameenginebook.com/) contains a [figure](https://www.gameenginebook.com/figures.html) with most if not all the Core features a game engine should contain. 
Sharp engine's first version should contain all the following components:
- Low-level Renderer
- Collision & Physics
- Audio
- Scene Graph / Culling Optimizations

Everything else in the figure should be implemented either by the game or will be implemeted into the engine later. 
Currently, the project is in it's infant phase where just the basic components have been implemented. The implemented components contain:
- Texture manager
- Shader manager
- Low-level renderer (excluding text & fonts)
- Scene graph
- Basic GUI renderer
- Threading

### Core and example game
The `Core` project is the actual game engine logic. The `Minecraft` project contains an example of what the engine is capable of and how a game can be built using the engine. 

Do note: The contributors of this repository do not have much experience when it comes to game engine / graphics development.
This repository is mainly a hobby project that is going to evolve over time.