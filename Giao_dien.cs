using ProcessData;
using ProcessData.Interfaces;
using System;
using System.Collections.Generic;
using Validate_Hotel;

namespace Quan_ly_khach_san
{
    public class Giao_dien
    {
        private BookingRepository bookingRepository = new BookingRepository();
        private RoomRepository roomRepo = new RoomRepository();
        private Validator validator = new Validate_Hotel.Validator();
        private BookingService bookingService;
        private BookingCancellationService cancellationService;

        public Giao_dien()
        {
            bookingService = new BookingService(roomRepo, validator);
            cancellationService = new BookingCancellationService(bookingRepository, roomRepo);
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("===== Quan ly khach san =====");
                Console.WriteLine("1. Dat phong");
                Console.WriteLine("2. Huy dat phong");
                Console.WriteLine("3. Hien thi danh sach dat phong");
                Console.WriteLine("4. Thoat");
                Console.Write("Chon chuc nang: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    if (choice == 1)
                    {
                        CreateBooking();
                    }
                    else if (choice == 2)
                    {
                        CancelBooking();
                    }
                    else if (choice == 3)
                    {
                        DisplayBookings();
                    }
                    else if (choice == 4)
                    {
                        Console.WriteLine("Thoat!");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Lua chon khong hop le. Vui long chon lai.");
                    }
                }
                else
                {
                    Console.WriteLine("Vui long nhap mot so.");
                }
            }
        }

        private void CreateBooking()
        {
            Console.Write("Nhap ten khach hang: ");
            string customerName = Console.ReadLine();

            Console.Write("Nhap ngay check-in (dd/MM/yyyy): ");
            DateTime checkIn = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", null);

            Console.Write("Nhap ngay check-out (dd/MM/yyyy): ");
            DateTime checkOut = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", null);

            Console.Write("Nhap so phong can dat: ");
            int roomCount = int.Parse(Console.ReadLine());

            var booking = new Booking
            {
                BookingId = new Random().Next(1, 1000), // Tạo ID ngẫu nhiên cho booking
                CustomerName = customerName,
                CheckInDate = checkIn,
                CheckOutDate = checkOut
            };

            for (int i = 0; i < roomCount; i++)
            {
                Console.Write($"Nhap so phong {i + 1}: ");
                int roomNumber = int.Parse(Console.ReadLine());
                var room = roomRepo.GetRoom(roomNumber);

                if (room != null && room.IsAvailable)
                {
                    room.IsAvailable = false; // Đánh dấu phòng đã được đặt
                    booking.Rooms.Add(room);
                }
                else
                {
                    Console.WriteLine($"Phong {roomNumber} khong co san hoac da duoc dat.");
                }
            }

            bookingRepository.AddBooking(booking);
            Console.WriteLine("Dat phong thanh cong!");
        }

        private void CancelBooking()
        {
            Console.Write("Nhap Booking ID de huy dat phong: ");
            int bookingId = int.Parse(Console.ReadLine());

            if (cancellationService.CancelBooking(bookingId))
            {
                Console.WriteLine("Huy dat phong thanh cong!");
            }
            else
            {
                Console.WriteLine("Khong tim thay dat phong voi Booking ID nay.");
            }
        }

        private void DisplayBookings()
        {
            var bookings = bookingRepository.GetAllBookings();

            if (bookings.Count == 0)
            {
                Console.WriteLine("Khong co dat phong nao.");
                return;
            }

            Console.WriteLine("===== Danh sach dat phong =====");
            foreach (var booking in bookings)
            {
                Console.WriteLine($"Booking ID: {booking.BookingId}, Ten khach hang: {booking.CustomerName}, Check-in: {booking.CheckInDate.ToShortDateString()}, Check-out: {booking.CheckOutDate.ToShortDateString()}");
                Console.WriteLine("Danh sach phong:");
                foreach (var room in booking.Rooms)
                {
                    Console.WriteLine($"- So phong: {room.RoomNumber}, Tinh trang: {(room.IsAvailable ? "Co san" : "Da dat")}");
                }
                Console.WriteLine();
            }
        }
        public static void Main(string[] args)
        {
            Giao_dien giaoDien = new Giao_dien();
            giaoDien.Run();
        }
    }
}
