import math

def get_float_input(prompt):
    """Безопасный ввод чисел с плавающей точкой"""
    while True:
        try:
            return float(input(prompt).replace(',', '.'))
        except ValueError:
            print("Ошибка: Пожалуйста, введите корректное число.")

def main():
    print("="*70)
    print("КОМПЛЕКСНЫЙ РАСЧЕТ СЛОВЕСНОЙ РАЗБОРЧИВОСТИ РЕЧИ")
    print("="*70)
    print("Программа выполняет расчеты по методике (пункты 1-5) и")
    print("рассчитывает итоговую словесную разборчивость Wc.\n")

    
    # Названия октав
    octaves = ["250", "500", "1000", "2000", "4000"]
    
    # Словари для хранения данных
    L_TCi = {}
    L_C_plus_Shi = {} 
    L_Shi = {}
    L_N = {}

    # L_1 = [88.68, 88.575, 86.23, 86.975, 76.225]
    # L_2 = [44.89, 40.81, 42.00, 40.56, 40.94]
    # L_3 = [23.43, 25.23, 28.12, 23.42, 21,32]

    L_1 = [73.68, 74.05, 74.99, 84.38, 73.78]
    L_2 = [52.96, 43.38, 43.10, 34.39, 44,24]
    L_3 = [32.74, 32.99, 31.89, 23.18, 32.91]

    L_4 = [66, 66, 61, 56, 53]
    
    for i in range(0,5):
        L_TCi[octaves[i]] = L_1[i]
        L_C_plus_Shi[octaves[i]] = L_2[i]
        L_Shi[octaves[i]] = L_3[i]
        L_N[octaves[i]] = L_4[i]


    print("\n" + "="*70)
    print("ЭТАП 1: РАСЧЕТ УРОВНЯ СИГНАЛА ЗА ПРЕГРАДОЙ (Lci)")
    print("="*70)
    
    L_Ci = {}
    
    for octave in octaves:
        lc_sh = L_C_plus_Shi[octave]
        lsh = L_Shi[octave]
        
        diff = lc_sh - lsh
        
        if diff >= 6:
            val = (10**(0.1 * lc_sh)) - (10**(0.1 * lsh))
            if val > 0:
                L_Ci[octave] = 10 * math.log10(val)
            else:
                L_Ci[octave] = lc_sh
            print(f"  {octave} Гц: Разница {diff:.2f} дБ >= 6 дБ. Расчет по формуле.")
        else:
            L_Ci[octave] = lc_sh - 7
            print(f"  {octave} Гц: Разница {diff:.2f} дБ < 6 дБ. Применен метод Lci = Lc+шi - 7 дБ.")
            
        print(f"    -> Lci = {L_Ci[octave]:.2f} дБ")

    print("\n" + "="*70)
    print("ЭТАП 2: РАСЧЕТ КОЭФФИЦИЕНТА ПРЕВЫШЕНИЯ (Delta_Li)")
    print("="*70)
    print("Delta_Li = L_TCi - L_Hi (формула из п.2 методики)")
    
    Delta_Li = {}
    
    for octave in octaves:
        delta_l = L_TCi[octave] - L_N[octave]
        Delta_Li[octave] = delta_l
        print(f"  {octave} Гц: Delta_Li = {L_TCi[octave]:.2f} - {L_N[octave]:.2f} = {delta_l:.2f} дБ")

    print("\n" + "="*70)
    print("ЭТАП 3: РАСЧЕТ ПРИВЕДЕННОГО УРОВНЯ (Lc_priv_i)")
    print("="*70)
    print("Lc_priv_i = Lci - Delta_Li (формула из п.3 методики)")
    
    Lc_priv_i = {}
    
    for octave in octaves:
        # Lc_priv_i = Lci - Delta_Li
        lc_priv = L_Ci[octave] - Delta_Li[octave]
        Lc_priv_i[octave] = lc_priv
        print(f"  {octave} Гц: Lc_priv_i = {L_Ci[octave]:.2f} - {Delta_Li[octave]:.2f} = {lc_priv:.2f} дБ")

    print("\n" + "="*70)
    print("ЭТАП 4: РАСЧЕТ ОТНОШЕНИЯ СИГНАЛ/ШУМ (Ei)")
    print("="*70)
    print("Ei = Lc_priv_i - L_шi (формула из п.4 методики)")
    
    Ei = {}
    
    for octave in octaves:
        e_val = Lc_priv_i[octave] - L_Shi[octave]
        Ei[octave] = e_val
        print(f"  {octave} Гц: Ei = {Lc_priv_i[octave]:.2f} - {L_Shi[octave]:.2f} = {e_val:.2f} дБ")
        
    print("\n" + "="*70)
    print("ЭТАП 5: РАСЧЕТ СЛОВЕСНОЙ РАЗБОРЧИВОСТИ (Wc)")
    print("="*70)
    
    sum_R = 0.0
    table_data = {
            "250": {"A": 18, "k": 0.03},
            "500": {"A": 14, "k": 0.12},
            "1000": {"A": 9, "k": 0.20},
            "2000": {"A": 6, "k": 0.30},
            "4000": {"A": 5, "k": 0.26}
        }
    
    for octave in octaves:
        A_i = table_data[octave]["A"]
        k_i = table_data[octave]["k"]
        
        Ei_db = Ei[octave]
        delta_i_raz = 10**(Ei_db / 20)
        
        Q_i = Ei_db - A_i
        
        term_exp = -4.3 * (10**-3) * ((27.3 - abs(Q_i))**2)
        numerator = 0.78 + 5.46 * math.exp(term_exp)
        denominator = 1 + 10**(0.1 * abs(Q_i))
        
        if Q_i <= 0:
            p_i = numerator / denominator
        else:
            p_i = 1 - (numerator / denominator)
            
        R_i = p_i * k_i
        sum_R += R_i
        
        print(f"  {octave} Гц:")
        print(f"    Ei (дБ) = {Ei_db:.2f} | Q_i = {Q_i:.2f}")
        print(f"    p_i = {p_i:.4f} | k_i = {k_i} | R_i = {R_i:.4f}")

    R = sum_R
    print(f"\nИнтегральный индекс артикуляции R = {R:.4f}")

    if R < 0.15:
        Wc = 1.54 * (R**0.25) * (1 - math.exp(-11 * R))
    else:
        Wc = 1 - math.exp((-11 * R) / (1 + 0.7 * R))

    print("="*70)
    print(f"ИТОГОВЫЙ РЕЗУЛЬТАТ: Словесная разборчивость речи Wc = {Wc:.4f}")
    
    if Wc > 0.9: quality = "Отлично слышно"
    elif Wc > 0.7: quality = "Хорошо слышно"
    elif Wc > 0.5: quality = "Удовлетворительно слышно"
    elif Wc > 0.3: quality = "Плохо слышно"
    else: quality = "Очень плохо слышно"
    
    print(f"Качество: {quality}")
    print("="*70)

if __name__ == "__main__":
    main()