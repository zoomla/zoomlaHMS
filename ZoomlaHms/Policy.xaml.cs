using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZoomlaHms.Common;

namespace ZoomlaHms
{
    /// <summary>
    /// Policy.xaml 的交互逻辑
    /// </summary>
    public partial class Policy : Window
    {
        private readonly Window owner;
        private readonly string policyText = @"
请务必认真阅读和理解本《软件许可使用协议》(以下简称《协议》)中规定的所有权利和限制。除非您接受本《协议》条款，否则您无权下载、安装或使用本“软件”及其相关服务。您一旦安装、复制、下载、访问或以其它方式使用本软件产品，将视为对本《协议》的接受，即表示您同意接受本《协议》各项条款的约束。如果您不同意本《协议》中的条款，请不要安装、复制或使用本软件。
1. 权利声明
本”软件”的一切知识产权，以及与”软件”相关的所有信息内容，包括但不限于：文字表述及其组合、图标、图饰、图像、图表、色彩、界面设计、版面框架、有关数据、附加程序、印刷材料或电子文档等均为作者所有，受著作权法和国际著作权条约以及其他知识产权法律法规的保护。
2. 许可范围
2.1 下载、安装和使用：本软件为免费软件，用户可以非商业性、无限制数量地下载、安装及使用本软件。
2.2 复制、分发和传播：用户可以非商业性、无限制数量地复制、分发和传播本软件产品。但必须保证每一份复制、分发和传播都是完整和真实的, 包括所有有关本软件产品的软件、电子文档, 版权和商标，亦包括本协议。
3. 权利限制
3.1 禁止反向工程、反向编译和反向汇编：用户不得对本软件产品进行反向工程(Reverse Engineer)、反向编译(Decompile)或反向汇编(Disassemble)，同时不得改动编译在程序文件内部的任何资源。除法律、法规明文规定允许上述活动外，用户必须遵守此协议限制。
3.2 组件分割：本软件产品是作为一个单一产品而被授予许可使用, 用户不得将各个部分分开用于任何目的。
3.3 个别授权：如需进行商业性的销售、复制、分发，包括但不限于软件销售、预装、捆绑等，必须获得作者的授权和许可。
3.4 保留权利：本协议未明示授权的其他一切权利仍归作者所有，用户使用其他权利时必须获得作者的书面同意。
4. 用户使用须知
4.1 本软件提供以HMS主题打包为代表的工具功能：主题打包，主题包内容同步，自动推送主题包到手机，主题包审计等功能。
4.2 本软件仅适用于Windows 7及以上、Windows Server 2012及以上操作系统。如果用户在安装本软件后因任何原因欲放弃使用，可删除本软件。
4.3 本软件由作者提供产品支持。
4.4 软件的修改和升级：作者保留为用户提供本软件的修改、升级版本的权利。
4.5 本软件不含有任何旨在破坏用户计算机数据和获取用户隐私信息的恶意代码，不含有任何跟踪、监视用户计算机的功能代码，不会监控用户网上、网下的行为，不会收集用户使用其它软件、文档等个人信息，不会泄漏用户隐私。
4.6 用户应在遵守法律及本协议的前提下使用本软件。用户无权实施包括但不限于下列行为：
4.6.1 不得删除或者改变本软件上的所有权利管理电子信息；
4.6.2 不得故意避开或者破坏著作权人为保护本软件著作权而采取的技术措施；
4.6.3 用户不得利用本软件误导、欺骗他人；
4.6.4 违反国家规定，对计算机信息系统功能进行删除、修改、增加、干扰，造成计算机信息系统不能正常运行；
4.6.5 未经允许，进入计算机信息网络或者使用计算机信息网络资源；
4.6.6 未经允许，对计算机信息网络功能进行删除、修改或者增加的；
4.6.7 未经允许，对计算机信息网络中存储、处理或者传输的数据和应用程序进行删除、修改或者增加；
4.6.8 破坏本软件系统或网站的正常运行，故意传播计算机病毒等破坏性程序；
4.6.9 其他任何危害计算机信息网络安全的。
4.7 对于从非作者指定站点下载的本软件产品以及从非作者发行的介质上获得的本软件产品，作者无法保证该软件是否感染计算机病毒、是否隐藏有伪装的特洛伊木马程序或者黑客软件，使用此类软件，将可能导致不可预测的风险，建议用户不要轻易下载、安装、使用，作者不承担任何由此产生的一切法律责任。
4.7.1 不得使用本软件发布违反国家法律的非法广告信息，如色情，赌博等，其造成的一切后果与本作者无关，请自觉营造和谐良性的网络营销环境。违法行为一经发现，本作者有权终止服务并追究法律责任。
4.8 隐私权保护：为了更好地改进软件和服务，在用户启动本软件时，本软件会向作者服务器报告软件所在计算机的.唯一标识，具体报告方法为访问服务器的一个页面，服务器根据该页面的被访问次数统计软件使用次数。作者不会将此数据与用户的个人身份信息相关联。
5. 免责与责任限制
5.1 本软件经过详细的测试，但不能保证与所有的软硬件系统完全兼容，不能保证本软件完全没有错误。如果出现不兼容及软件错误的情况，用户可登录软件官网论坛将情况报告作者，获得技术支持。如果无法解决兼容性问题，用户可以删除本软件。
5.2 使用本软件产品风险由用户自行承担，在适用法律允许的最大范围内，对因使用或不能使用本软件所产生的损害及风险，包括但不限于直接或间接的个人损害、商业赢利的丧失、贸易中断、商业信息的丢失或任何其它经济损失，作者不承担任何责任。
5.3 对于因电信系统或互联网网络故障、计算机故障或病毒、信息损坏或丢失、计算机系统问题或其它任何不可抗力原因而产生损失，作者不承担任何责任。
5.4 用户违反本协议规定，对作者公司造成损害的。作者有权采取包括但不限于中断使用许可、停止提供服务、限制使用、法律追究等措施。
6. 法律及争议解决
6.1 本协议适用中华人民共和国法律。
6.2 因本协议引起的或与本协议有关的任何争议，各方应友好协商解决；协商不成的，任何一方均可将有关争议提交至江西仲裁委员会并按照其届时有效的仲裁规则仲裁；仲裁裁决是终局的，对各方均有约束力。
7. 其他条款
7.1 如果本协议中的任何条款无论因何种原因完全或部分无效或不具有执行力，或违反任何适用的法律，则该条款被视为删除，但本协议的其余条款仍应有效并且有约束力。
7.2 作者有权根据有关法律、法规的变化以及公司经营状况和经营策略的调整等修改本协议。修改后的协议会随附于新版本软件。当发生有关争议时，以最新的协议文本为准。如果不同意改动的内容，用户可以自行删除本软件。如果用户继续使用本软件，则视为您接受本协议的变动。
7.3 本协议的一切解释权与修改权归作者所有。
";

        private bool onlyView = false;

        public Policy(Window owner)
        {
            this.owner = owner;
            InitializeComponent();

            TextRange text = new TextRange(Content.Document.ContentStart, Content.Document.ContentEnd);
            text.Text = policyText.Trim();
        }

        public Policy() : this(null)
        {
            onlyView = true;
        }

        #region 同意/拒绝功能实现
        private void Window_Closed(object sender, EventArgs e)
        {
            if (onlyView)
            { return; }

            if (File.Exists(System.IO.Path.Combine(SystemPath.StartupLocation, ".agreement")))
            { owner.Show(); }
            else
            { Application.Current.Shutdown(); }
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            if (onlyView)
            { return; }

            Application.Current.Shutdown();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (onlyView)
            { return; }

            File.Create(System.IO.Path.Combine(SystemPath.StartupLocation, ".agreement"));
            Close();
        }
        #endregion

        bool l_altOk, r_altOk, qOk, aOk, cOk;

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            l_altOk = r_altOk = qOk = aOk = cOk = false;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.SystemKey)
            {
                case Key.LeftAlt:
                    l_altOk = true;
                    break;
                case Key.Right:
                    r_altOk = true;
                    break;
                case Key.Q:
                    qOk = true;
                    break;
                case Key.A:
                    aOk = true;
                    break;
                case Key.C:
                    cOk = true;
                    break;
                default:
                    break;
            }

            if (l_altOk || r_altOk)
            {
                if (qOk && !onlyView)
                {
                    RejectButton_Click(null, null);
                }
                else if (aOk && !onlyView)
                {
                    AcceptButton_Click(null, null);
                }
                else if (cOk && onlyView)
                {
                    Close();
                }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.SystemKey)
            {
                case Key.LeftAlt:
                    l_altOk = false;
                    break;
                case Key.Right:
                    r_altOk = false;
                    break;
                case Key.Q:
                    qOk = false;
                    break;
                case Key.A:
                    aOk = false;
                    break;
                case Key.C:
                    cOk = false;
                    break;
                default:
                    break;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (onlyView)
            {
                AcceptButton.Visibility = Visibility.Collapsed;
                RejectButton.Visibility = Visibility.Collapsed;
                CloseButton.Visibility = Visibility.Visible;
            }
            else
            {
                CloseButton.Visibility = Visibility.Collapsed;
                RejectButton.Visibility = Visibility.Visible;
                AcceptButton.Visibility = Visibility.Visible;
            }
        }
    }
}
