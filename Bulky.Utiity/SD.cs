using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Utility
{
    public static class SD
    {
        public const string Role_Custmoer = "客户";
        public const string Role_Company = "公司";
        public const string Role_Admin = "管理员";
        public const string Role_Employee = "员工";

        public const string StatusPending = "待处理";
        public const string StatusApproved = "已批准";
        public const string StatusInProcess = "处理中";
        public const string StatusShipped = "已发货";
        public const string StatusCancelled = "已取消";
        public const string StatusRefunded = "已退款";

        public const string PaymentStatusPending = "待处理";
        public const string PaymentStatusApproved = "已批准";
        public const string PaymentStatusDelayedPayment = "延迟付款";
        public const string PaymentStatusRejected = "已拒绝";
    }
}
