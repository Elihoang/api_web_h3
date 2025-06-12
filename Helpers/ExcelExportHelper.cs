using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using API_WebH3.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using API_WebH3.Models;
using System.Globalization;

namespace API_WebH3.Helper
{
    public class ExcelExportHelper
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ICouponRepository _couponRepository;

        public ExcelExportHelper(
            IOrderRepository orderRepository,
            ICourseRepository courseRepository,
            ICouponRepository couponRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _couponRepository = couponRepository ?? throw new ArgumentNullException(nameof(couponRepository));
        }

        public async Task<byte[]> GenerateOrderReportAsync(DateTime? startDate, DateTime? endDate, string period = "all", int? month = null, int? year = null)
        {
            if (string.IsNullOrEmpty(period)) period = "all";

            // Lấy danh sách đơn hàng
            var orders = await _orderRepository.GetAllAsync() ?? new List<Order>();
            if (startDate.HasValue || endDate.HasValue || period != "all" || month.HasValue || year.HasValue)
            {
                orders = orders.Where(order =>
                {
                    if (string.IsNullOrEmpty(order.CreatedAt)) return false;
                    if (!DateTime.TryParseExact(order.CreatedAt, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime orderDate)) return false;

                    // Lọc theo khoảng thời gian
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        var adjustedEndDate = endDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                        return orderDate >= startDate.Value && orderDate <= adjustedEndDate;
                    }
                    // Lọc theo ngày cụ thể
                    else if (startDate.HasValue)
                    {
                        return orderDate.Date == startDate.Value.Date;
                    }
                    // Lọc theo tháng/năm cụ thể
                    else if (period == "month" && month.HasValue && year.HasValue)
                    {
                        return orderDate.Month == month.Value && orderDate.Year == year.Value;
                    }
                    // Lọc theo tháng hiện tại
                    else if (period == "month")
                    {
                        return orderDate.Month == DateTime.Now.Month && orderDate.Year == DateTime.Now.Year;
                    }
                    return true;
                }).ToList();
            }

            // Tạo workbook và sheet
            using (var memoryStream = new MemoryStream())
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Báo Cáo Đơn Hàng");

                // Tạo header row
                IRow headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Mã Đơn Hàng");
                headerRow.CreateCell(1).SetCellValue("Họ Tên");
                headerRow.CreateCell(2).SetCellValue("Khóa Học");
                headerRow.CreateCell(3).SetCellValue("Số Tiền");
                headerRow.CreateCell(4).SetCellValue("Mã Giảm Giá");
                headerRow.CreateCell(5).SetCellValue("Số Tiền Giảm");
                headerRow.CreateCell(6).SetCellValue("Trạng Thái");
                headerRow.CreateCell(7).SetCellValue("Ngày Tạo");

                // Định dạng tiền tệ
                var currencyFormat = workbook.CreateDataFormat();
                var currencyStyle = workbook.CreateCellStyle();
                currencyStyle.DataFormat = currencyFormat.GetFormat("#,##0 ₫");

                // Điền dữ liệu
                int rowIndex = 1;
                foreach (var order in orders)
                {
                    var orderDetails = await _orderRepository.GetOrderDetailsByOrderIdAsync(order.Id);
                    foreach (var detail in orderDetails)
                    {
                        IRow dataRow = sheet.CreateRow(rowIndex++);
                        dataRow.CreateCell(0).SetCellValue(order.Id ?? "N/A");
                        dataRow.CreateCell(1).SetCellValue(order.User?.FullName ?? "Không Xác Định");
                        var courseTitle = detail.CourseId != null ? await GetCourseTitle(detail.CourseId) : "Không Xác Định";
                        dataRow.CreateCell(2).SetCellValue(courseTitle);

                        // Định dạng số tiền
                        var amountCell = dataRow.CreateCell(3);
                        amountCell.SetCellValue((double)detail.Price);
                        amountCell.CellStyle = currencyStyle;

                        // Mã giảm giá
                        var couponCode = detail.CouponId.HasValue ? await GetCouponCode(detail.CouponId.Value) : "Không Áp Dụng";
                        dataRow.CreateCell(4).SetCellValue(couponCode);

                        // Số tiền giảm
                        var discountCell = dataRow.CreateCell(5);
                        discountCell.SetCellValue((double)(detail.DiscountAmount ?? 0));
                        discountCell.CellStyle = currencyStyle;

                        dataRow.CreateCell(6).SetCellValue(order.Status ?? "N/A");
                        dataRow.CreateCell(7).SetCellValue(order.CreatedAt ?? "N/A");
                    }
                }

                // Tự động điều chỉnh độ rộng cột
                for (int i = 0; i < 8; i++)
                {
                    sheet.AutoSizeColumn(i);
                }

                workbook.Write(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private async Task<string> GetCourseTitle(string courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            return course?.Title ?? "Không Xác Định";
        }

        private async Task<string> GetCouponCode(Guid couponId)
        {
            var coupon = await _couponRepository.GetByIdAsync(couponId);
            return coupon?.Code ?? "Không Xác Định";
        }
    }
}