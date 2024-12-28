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

## Contribution
If you wish to contribute to the project, you should create a new feature branch off of main and create a pull request with your changes so that the main contributors are able to make sure your contributions are on an acceptable level of quality.
When contributing to the project, always create new issues about your changes with appropriate descriptions of the changes you are about to make.

Additionally your branch name should be in the following format: `<issue number>-<your initials>-<some name>`. E.g. `1234-av-renderer-optimizations`.

For any question, please reach out to the owner(s) of the repository.

## Material and insipiration from:
- [Learn OpenGL](https://learnopengl.com/)    
- [Learn OpenTK - OpenTK examples](https://opentk.net/learn/index.html) (examples ported from the Learn OpenGL website)  
- [OpenGL Wiki](https://www.khronos.org/opengl/wiki)  
- [OpenGL API Documentation](https://docs.gl/)  
- [Erfan Ahmadi - Rendering Engine Development](https://gist.github.com/Erfan-Ahmadi/defe4ab99af97f624b68e0dccb0712ea)  
- [Game Engine Architecture by Jason Gregory](https://www.gameenginebook.com/)  
- [Imgui repository](https://github.com/ocornut/imgui)  
- [Shadertoy website](https://www.shadertoy.com/)  
- [Game engine architecture 2nd edition overview, Chapter 1, pt1](http://hightalestudios.com/2017/03/game-engine-architecture-2nd-edition-overview-ch-1/)  
- [Game engine architecture 2nd edition overview, Chapter 1, pt2](http://hightalestudios.com/2017/03/game-engine-architecture-2nd-edition-overview-ch-1-part-2/)  
- [Ray traycing in one weekend - course material](https://raytracing.github.io/books/RayTracingInOneWeekend.html#overview)  
- [Scratchpixel - graphics programming lessons](https://www.scratchapixel.com/index.html)  
- [Wwise fundamentals](https://www.audiokinetic.com/en/library/edge/?source=WwiseFundamentalApproach&id=wwise_fundamentals)  
- [VoxelRifts - A Brief look at Text Rendering](https://www.youtube.com/watch?v=qcMuyHzhvpI)  
- [Freya Holmér - The Continuity of Splines](https://www.youtube.com/watch?v=jvPPXbo87ds)  
- [Sebastian Lague - Coding Adventure: Rendering Text](https://www.youtube.com/watch?v=SO83KQuuZvg)
- [Sharp engine](https://sharp-engine.com/)  
- https://developer.nvidia.com/industries/game-development  
- https://developer.nvidia.com/gpugems/gpugems3/contributors  
- https://developer.nvidia.com/graphics-research-tools  
- https://developer.nvidia.com/industries/aeco  
- https://gdcvault.com/browse/  
- https://developer.nvidia.com/gpugems/gpugems3/part-iv-image-effects/chapter-25-rendering-vector-art-gpu  
- https://blog.siggraph.org/2024/12/our-top-10-most-read-blogs-of-2024.html/  
