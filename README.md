# 鲶鱼精邮差

鲶鱼精邮差是一个用于拓展ACT与相关插件的功能的插件。可以接收Triggernometry高级触发器等插件传递过来的游戏文本指令，并投递至游戏内执行。  
仅限于最终幻想14自己支持的、可以在游戏内由宏或由文本聊天栏中输入并执行的指令。如/em(自定义情感动作) /greet(打招呼) /focustarget(显示焦点目标)等，不支持任何游戏本身不提供的文本指令。  
可以用于解决目前游戏内现有宏指令系统的一些不便之处，例如复活喊话宏、翻页宏、死而不僵提醒宏等。

### 主要特性
- 执行指令不会打断游戏内宏的运行，也不会污染聊天框历史记录。
- 自动识别与支持国服与国际服最新版本的游戏客户端。在小版本更新后可以不受影响继续使用。
- ACT插件形式，启动ACT时自动加载，无需单独启动其他程序。
- 可以自动识别并切换当前解析插件对应的游戏进程。在退出、重启游戏后不需要手动操作即可自动完成切换。
- 兼容PaisleyPark的标点指令，可以进行本地标点（不支持保存、导出标点等PaisleyPark的其他功能）

### 注意事项

插件自身不含有直接执行指令的能力，单独使用时无法行使任何功能，必须配合ACT与Triggernometry高级触发器等工具才可以发挥作用。  
使用此工具需要对ACT、高级触发器以及游戏内的文本指令有较深的理解与使用基础，本文不提供相关教程，还请自行查阅相关资料。  
使用此工具进行的任何操作皆需遵守相关规定，使用造成风险由您承担。  
**游戏大版本更新后可能导致游戏崩溃，需要等待更新后再使用。**  
需要至少.NET Framework 4.6.1版本的环境支持，仅支持DX11客户端。

### 安装方法
鲶鱼精邮差为ACT插件，需要使用ACT加载使用。  
将下载后的压缩包解压后，通过ACT的插件列表页添加PostNamazu.dll插件并启用，即可在插件栏看到鲶鱼精邮差的面板。  
> 2个dll文件需放在同一目录下。

### 使用方法
启动程序后，**设置端口**并点击“启动”开始在指定端口监听。  
勾选*自动启动*选项后，每次启动ACT加载插件完毕后鲶鱼精邮差会自动启动监听。  
<details>
<summary>鲶鱼精邮差设置</summary>
<img width="600" src="https://github.com/Natsukage/Assets/blob/main/PostNamazu/images/%E9%B2%B6%E9%B1%BC%E7%B2%BE%E8%AE%BE%E7%BD%AE1.png"/>
</details>  

在ACT的Triggernometry高级触发器中添加触发器，并将动作类型选择为“通用JSON动作”，端点URL设置为`http://127.0.0.1:你设置的端口/command`  
有效负载发送设置为你要执行的文本指令，例如`/e 123`。测试触发后如果在游戏内看到提示文字即为配置成功。  
<details>
<summary>Triggernometry高级触发器设置</summary>
<img width="600" src="https://github.com/Natsukage/Assets/blob/main/PostNamazu/images/%E9%AB%98%E7%BA%A7%E8%A7%A6%E5%8F%91%E5%99%A8%E8%AE%BE%E7%BD%AE.png"/>
</details>  

如果有多个FF14游戏进程存在，鲶鱼精邮差会自动匹配解析插件当前对应的游戏进程。可以在解析插件的面板中进行切换。
<details>
<summary>切换方式</summary>
<img width="600" src="https://github.com/Natsukage/Assets/blob/main/PostNamazu/images/%E9%B2%B6%E9%B1%BC%E7%B2%BE%E8%AE%BE%E7%BD%AE2.png"/>
</details>

### 应用场景举例
#### 翻页宏
当特定条件触发时执行/hotbar set或/hotbar copy指令将指定键位设置为指定技能。

<details>
<summary>发动即刻咏唱后，将即刻的键位替换为复活</summary>
<img width="600" src="https://github.com/Natsukage/Assets/blob/main/PostNamazu/images/%E5%8D%B3%E5%88%BB.gif"/>
<img width="600" src="https://github.com/Natsukage/Assets/blob/main/PostNamazu/images/%E6%8A%80%E8%83%BD%E6%9B%BF%E6%8D%A24.png"/>
<img width="600" src="https://github.com/Natsukage/Assets/blob/main/PostNamazu/images/%E6%8A%80%E8%83%BD%E6%9B%BF%E6%8D%A25.png"/>
</details>

由于翻页的触发条件限定为即刻咏唱使用成功获得即刻咏唱状态，因此在即刻cd时、自身硬直中等情况下，即使激情连打，只要没有成功的用出即刻，就不会误触发翻页。  
并且即刻与复活都为原本的技能而非宏实现，可以正常进入施法队列。  

<details>
<summary>原 初 的 解 放！！！</summary>
<img width="600" src="https://github.com/Natsukage/Assets/blob/main/PostNamazu/images/%E9%94%AF%E7%88%861.png"/>
<img width="600" src="https://github.com/Natsukage/Assets/blob/main/PostNamazu/images/%E9%94%AF%E7%88%862.png"/>
</details>  
同理，可以实现在成功召唤炽天使后将炽天使召唤替换为慰藉，黑魔在转状态后将火系技能栏替换为冰系技能栏，boss读条魔之符文时将暗影锋替换为至黑之夜等，原理基本相同，这里不多赘述。


#### 喊话宏
当特定条件触发时执行`/s`(公频白字发言) `/p`(小队内发言) `/linkshell2`(第二个通讯贝内发言)等指令进行发言。  

<details>
<summary>当触发死而不僵时，在小队频道内喊“用力奶我！&lt;se.8&gt;</summary>
<img width="600" src="https://github.com/Natsukage/Assets/blob/main/PostNamazu/images/%E6%AD%BB%E8%80%8C%E4%B8%8D%E5%83%B5.gif"/>
<img width="600" src="https://github.com/Natsukage/Assets/blob/main/PostNamazu/images/%E6%AD%BB%E8%80%8C%E4%B8%8D%E5%83%B52.png"/>
</details>

由于触发条件为获得buff，因此在按下活死人后如果没有触发死而不僵，也就不会喊话。  
由于喊话的时点为死而不僵的触发时点，相对来说也更加方便计时。  
并且因为释放技能非宏实现，所以有效避免了无敌卡宏的情况。  

<details>
<summary>复活喊话，由于死人不太好找这里就先用医术作例子了</summary>
<img width="600" src="https://github.com/Natsukage/Assets/blob/main/PostNamazu/images/%E6%8A%80%E8%83%BD%E5%96%8A%E8%AF%9D%E6%8F%90%E7%A4%BA1.png"/>
<img width="600" src="https://github.com/Natsukage/Assets/blob/main/PostNamazu/images/%E6%8A%80%E8%83%BD%E5%96%8A%E8%AF%9D%E6%8F%90%E7%A4%BA2.png"/>
</details>

同理，可以实现挑衅退避时队内发言，找到怪物后在部队频道内通报坐标，鱼王上钩时第一时间~~晒~~转发至通讯贝内等，原理基本相同，这里不多赘述。

#### 更多进阶应用
<details>
<summary>按下超火流星后，点掉超火流星，并在小队频道发送：“？(倒置)”</summary>
<img width="600" src="https://github.com/Natsukage/Assets/blob/main/PostNamazu/images/%E8%B6%85%E7%81%AB%E6%B5%81%E6%98%9F1.png"/>
<img width="600" src="https://github.com/Natsukage/Assets/blob/main/PostNamazu/images/%E8%B6%85%E7%81%AB%E6%B5%81%E6%98%9F2.jpg"/>
</details>

收到来自特定玩家的组队邀请时自动发送/join指令接受邀请。  
收到其他玩家发起的准备确认时自动发送/ready指令进行确认  
……  
实现方法非常简单，大家可以自行举一反三，这里不多赘述。

### PaisleyPark兼容
鲶鱼精邮差兼容PaisleyPark的标点指令，可以接收触发器发送的坐标JSON字符串进行本地标点。  
使用方式与原版PaisleyPark相同。将PaisleyPark适用的标点指令文本发送至`http://127.0.0.1:你设置的端口/place`即可。  
文本指令与标点指令使用相同的端口，区别在于后面跟随的路径（/command /place）。
只要端口号正确对应，不需修改即可继续使用原版PaisleyPark的触发指令。  
具体使用方式请参考PaisleyPark的相关教程。

### 冲突
在Triggernometry高级触发器中建立将指令发送给鲶鱼精邮差的非异步（没有勾选计划任务页的“异步执行，不会阻止执行其他操作”选项的）触发器时，点击主界面的Test Action将会造成ACT假死直至超时（持续数分钟）  
这是由于在Triggernometry中测试非异步触发器时，触发器将会使用ACT的当前主进程进行触发器的测试，并且在获得反馈结果之前将会冻结ACT主进程阻止后续操作。因此同为ACT插件的邮差也会被阻塞，无法接收到Triggernometry的触发指令并进行反馈，造成死锁。此状况将会一直持续直至Triggernometry的操作由于超时而被中断。  
此情况仅会出现于手动点击Test Action进行触发器测试的场合。当触发器由游戏内日志行正常触发时，无论是同步还是异步执行的触发器都不会阻塞ACT的主线程，也不会发生上述的死锁现象。此外，对于异步触发器，即使通过在触发器页面手动点击Test Action进行触发器测试，Triggernometry也会在新建立的线程中执行触发器操作，而不会阻塞ACT主进程。因此同样不会造成死锁。  
综上，此问题并不会影响鲶鱼精邮差与Triggernometry在游戏中的正常使用（无论是同步还是异步触发器操作）。但是在测试自己建立的触发器时，为了防止上述情况发生，建议尽量将与邮差进行交互的触发器操作设置为异步执行，或通过游戏内日志的触发方式对触发器进行测试。

### 感谢
感谢 [@PrototypeSeiren](https://github.com/PrototypeSeiren)[@Bluefissure](https://github.com/Bluefissure)[@DieMoe233](https://github.com/DieMoe233)各位大佬的付出与帮助。  
