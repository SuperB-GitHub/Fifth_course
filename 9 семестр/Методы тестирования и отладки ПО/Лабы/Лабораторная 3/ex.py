from services import nextday

print("Добрый день! Добро пожаловать в программу для расчета следующего дня!")
year = int(input("Введите год: "))
month = int(input("Введите месяц: "))
day = int(input("Введите день: "))

if __name__ == "__main__":
    # БЫЛО: nextday.next_day(year, month, day) — порядок аргументов неверный
    # СТАЛО:
    print(f'Следующим днем будет: {nextday.next_day(day, month, year)}')