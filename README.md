# tokyo
使用C#实现的软光栅渲染器(Soft Renderer)，支持深度缓冲，背面剔除。

## 渲染模式
支持线框模式（WireFrameShading.cs）

![wireframe](https://github.com/linyuhe/tokyo/blob/master/image/wireframe.jpg?raw=true)

支持平面着色模式(FlatShading.cs)

![flat](https://github.com/linyuhe/tokyo/blob/master/image/flat.jpg?raw=true)

支持高式着色模式(PhongShading.cs)

![Phong](https://github.com/linyuhe/tokyo/blob/master/image/Phong.jpg?raw=true)

支持纹理着色模式(TextureShading.cs)

![texture](https://github.com/linyuhe/tokyo/blob/master/image/texture.jpg?raw=true)

光线追踪-反射

![reflection](https://github.com/linyuhe/tokyo/blob/master/image/reflection.jpg?raw=true)

使用SDFAABB抗锯齿

![sdfaabb](https://github.com/linyuhe/tokyo/blob/master/image/sdfaabb.jpg?raw=true)

## 参考文档
[3D软件渲染器入门](https://www.kancloud.cn/digest/soft-3d-engine)

[用JavaScript玩转计算机图形学(一)光线追踪入门](http://www.cnblogs.com/miloyip/archive/2010/03/29/1698953.html)

[用 C 语言画直线](https://zhuanlan.zhihu.com/p/30553006)