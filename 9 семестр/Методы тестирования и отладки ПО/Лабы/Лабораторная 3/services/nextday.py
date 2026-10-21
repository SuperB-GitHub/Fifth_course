# services/nextday.py

def is_leap_year(year):
    """Check if a year is a leap year."""
    return (year % 4 == 0 and year % 100 != 0) or (year % 400 == 0)


def days_in_month(month, year):
    """Return the number of days in a given month."""
    if month in [1, 3, 5, 7, 8, 10, 12]:
        return 31
    elif month in [4, 6, 9, 11]:
        return 30
    elif month == 2:
        return 29 if is_leap_year(year) else 28
    else:
        return None


def next_day(day, month, year):
    # Validate input
    if not isinstance(day, int) or not isinstance(month, int) or not isinstance(year, int):
        return None

    if year < 1 or month < 1 or month > 12 or day < 1:
        return None

    max_days = days_in_month(month, year)
    if max_days is None or day > max_days:
        return None

    # Calculate next day
    if day < max_days:
        return (day + 1, month, year)
    elif month < 12:
        return (1, month + 1, year)
    else:
        return (1, 1, year + 1)