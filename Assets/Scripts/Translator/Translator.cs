using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Translator : MonoBehaviour
{
    public TMP_FontAsset newFont;
    public TextMeshProUGUI[] allTextComponents;
    public enum Language { Kazakh, English, Russian }

    public Language currentLanguage = Language.Russian;

    void Start()
    {
        foreach (var textComponent in allTextComponents)
        {
            if (textComponent.name != "KazakhText" && textComponent.name != "EnglishText")
            {
                CreateLanguageTexts(textComponent);
            }
        }

        SetLanguage(currentLanguage);
    }

    private void CreateLanguageTexts(TextMeshProUGUI originalText)
    {
        string baseText = originalText.text;

        GameObject kazakhTextObject = new GameObject("KazakhText");
        kazakhTextObject.transform.SetParent(originalText.transform, false);
        TextMeshProUGUI kazakhText = kazakhTextObject.AddComponent<TextMeshProUGUI>();
        CopyTextProperties(originalText, kazakhText);
        kazakhText.text = TranslateToKazakh(baseText);
        kazakhText.enabled = false;

        GameObject englishTextObject = new GameObject("EnglishText");
        englishTextObject.transform.SetParent(originalText.transform, false);
        TextMeshProUGUI englishText = englishTextObject.AddComponent<TextMeshProUGUI>();
        CopyTextProperties(originalText, englishText);
        englishText.text = TranslateToEnglish(baseText);
        englishText.enabled = false;
    }

    private void CopyTextProperties(TextMeshProUGUI source, TextMeshProUGUI target)
    {
        target.font = newFont;
        target.fontSize = source.fontSize;
        target.color = source.color;
        target.alignment = source.alignment;
        target.rectTransform.anchoredPosition = source.rectTransform.anchoredPosition;
        target.rectTransform.sizeDelta = source.rectTransform.sizeDelta;
        target.rectTransform.anchorMin = source.rectTransform.anchorMin;
        target.rectTransform.anchorMax = source.rectTransform.anchorMax;
        target.rectTransform.pivot = source.rectTransform.pivot;

        //// Копирование масштаба, поворота и позиции
        //target.rectTransform.localScale = source.rectTransform.localScale;
        //target.rectTransform.localRotation = source.rectTransform.localRotation;
        target.rectTransform.localPosition = new Vector3(0f,0f,0f);
    }

    private string TranslateToKazakh(string originalText)
    {
        return originalText switch
        {
            "Остановить" => "Тоқтату",
            "Сохранить" => "Сақтау",
            "Трубы" => "Құбырлар",
            "Установки" => "Орнатулар",
            "Вспомогательные" => "Көмекші",
            "ОПИСАНИЕ" => "СИПАТТАМАСЫ",
            "ВЫХОД" => "ШЫҒУ",
            "ЦИФРОВОЙ ДВОЙНИК КОМПЛЕКСНОЙ СИСТЕМЫ" => "КҮРДЕЛІ ЖҮЙЕНІҢ ЦИФРЛЫҚ ЕКІЗІ",
            "Настройка системы" => "Жүйені орнату",
            "Печка" => "Пеш",
            "Водяной Эмульгатор" => "Су эмульгаторы",
            "Реагентный Эмульгатор" => "Реагент эмульгаторы",
            "Сбор CO2" => "CO2 жинау",
            "Таблица" => "Кесте",
            "Состав газа" => "Газ құрамы",
            "Плотность" => "Тығыздығы",
            "Температура" => "Температура",
            "Состав жидкости" => "Сұйықтық құрамы",
            "Скорость" => "Жылдамдық",
            "Заряд" => "Заряд",
            "Радиус частиц " => "Бөлшек радиусы",
            "Кол. Катал. 1" => "Кол. Катал. 1",
            "Кол. Катал. 2" => "Кол. Катал. 2",
            "Давление" => "Қысым",
            "Скорость потока" => "Ағын жылдамдығы",
            "Расход газа" => "Газды тұтыну",
            "Расход жидкости" => "Сұйықтық ағыны",
            "Тип сорбента" => "Сорбент түрі",
            "Объем газа" => "Газ көлемі",
            "Длина гидравли. диаметра" => "Гидр. диаметрдің ұзындығы",
            "Скороть вентилятора" => "Желдеткіш жылдамдығы",
            "Компонент" => "Құрамдас",
            "Пыль" => "Шаң",
            "Твердые частицы" => "Бөлшекті заттар",
            "Разрешение" => "РАЗРЕШЕНИЕ",
            "Зола" => "Күл",
            "Сажа" => "Күйе",
            "Массовый поток газа" => "Газ массасы ағыны",
            "Электрическое поле" => "Электр өрісі",
            "Ускорение частиц" => "Бөлшектердің үдеуі",
            "Скорость газа" => "Газ жылдамдығы",
            "Скорость воды" => "Су жылдамдығы",
            "Массовый поток жидкости" => "Судың массалық ағыны",
            "Коэф. массового переноса" => "Масса алмасу коэффициенті",

            "Система охлаждения" => "Салқындату жүйесі",
            "Котел" => "Котел",
            "Электрические фильтры" => "Электрлік" + "\n" + "сүзгілер",
            "Предварительная термическая подготовка" => "Алдын ала " + "\n" + "термиялық" + "\n" + "дайындық",
            "Блок Каталитической очистки" => "Каталитикалық " + "\n" + "тазарту" + "\n" + "қондырғысы",
            "1-я ступень" => "1-ші-кезең",
            "2-я ступень" => "2-ші кезең",
            "3-я ступень" => "3-ші кезең",
            "Выход" => "Шығу",
            "Производство с применением золы" => "Күлді қолдану" + "\n" + "арқылы өндіру",
            "Удобрение" => "Тыңайтқыш",
            "Отгрузка товарной углекислоты" => "Коммерциялық көмірқышқыл" + "\n" + " газын жөнелту",
            "Удаление сажи и ТЦ" => "Күйе мен ТК жою",
            "Удаление СО" => "CO-ны жою",
            "Удаление остатков сажи и ТЦ" => "Күйе мен ТК қалдықтарын жою",
            "Удаляется SO, NO2" => "SO, NO2 жояды",
            "Сбор СО2" => "CO2 жинау",
            "Очистной комплекс" => "Тазалау кешені",
            "Регенерация реагента" => "Реагент" + "\n" + "регенерациясы",
            "Дымовые газы" => "Түтін газдары",
            "Пульт управления" => "Басқару тақтасы",
            "Горячий газ" => "Ыстық газ",
            "Компрессор КД" => "КД" + "\n" + "компрессоры",
            "Компрессор ВД" => "ВД" + "\n" + "компрессоры",
            "Запуск" => "Қосу",
            "Стоп" => "Стоп",
            "Пауза" => "Кідірту",
            "Назад" => "Артқа",
            "Частицы" => "Бөлшектер",
            "Напряжение" => "Кернеу",
            "Адсорбер 1" => "Адсорбер 1",
            "Адсорбер 2" => "Адсорбер 2",
            "Объем газа - V, м3/ч" => "Газ көлемі - V, м3/сағ",
            "Температура адцорбции - T, °C" => "Адсорбция температурасы - T, °C",
            "Температура дисорбции - T, °C" => "Дисорбция температурасы - T, °C",
            "Длина гидравли. диаметра - D, м" => "Гидравликалық ұзындық. диаметрі - D, м",
            "Скороть вентилятора - об/мин" => "Желдеткіш жылдамдығы - айн/мин",


            _ => originalText 
        };
    }

    private string TranslateToEnglish(string originalText)
    {
        return originalText switch
        {
            "Остановить" => "Stop",
            "Сохранить" => "Save",
            "Трубы" => "Pipes",
            "Установки" => "Units",
            "Вспомогательные" => "Auxiliary",
            "ВЫХОД" => "EXIT",
            "ЦИФРОВОЙ ДВОЙНИК КОМПЛЕКСНОЙ СИСТЕМЫ" => "DIGITAL TWIN OF THE COMPLEX SYSTEM",
            "Настройка системы" => "System Setup",
            "Печка" => "Stove",
            "Водяной Эмульгатор" => "Water Emulsifier",
            "Реагентный Эмульгатор" => "Reagent Emulsifier",
            "Сбор CO2" => "CO2 Collection",
            "Таблица" => "Table",
            "Состав газа" => "Gas Composition",
            "Плотность" => "Density",
            "Заряд" => "Charge",
            "Скорость" => "Velocity",
            "Температура" => "Temperature",
            "Состав жидкости" => "Liquid Comp.",
            "Радиус частиц" => "Particle radius",
            "Кол. Катал. 1" => "Num Catal. 1",
            "Кол. Катал. 2" => "Num Catal. 2",
            "Давление" => "Pressure",
            "Скорость потока" => "Flow rate",
            "Расход газа" => "Gas flow rate",
            "Расход жидкости" => "Liquid flow rate",
            "Тип сорбента" => "Sorbent type",
            "Объем газа" => "Gas volume",
            "Длина гидравли. диаметра" => "hydraulic lenght diameter",
            "Скороть вентилятора" => "Fan speed",
            "Компонент" => "Component",
            "Пыль" => "Dust",
            "Твердые частицы" => "Particulate matter",
            "Зола" => "Ash",
            "Сажа" => "Soot",
            "Массовый поток газа" => "Gas mass flow",
            "Электрическое поле" => "Electric Field",
            "Ускорение частиц" => "Particle acceleration",
            "Скорость газа" => "Gas velocity",
            "Скорость воды" => "Water velocity",
            "Массовый поток жидкости" => "Mass flow of liquid",
            "Коэф. массового переноса" => "Mass transfer coefficient",
            "Электрофильтр" => "Electrofilter",
            "СИМУЛЯЦИЯ" => "SIMULATION",
            "КОНСТРУКТОР" => "CONSTRUCTOR",
            "Разрешение" => "RESOLUTION",
            "Катализатор" => "Catalyst",
            "ОПИСАНИЕ" => "DESCRIPTION",
            "Симулировать" => "Simulate",

            "Система охлаждения" => "Cooling system",
            "Котел" => "Boiler",
            "Электрические фильтры" => "Electrical" + "\n" + "filters",
            "Предварительная термическая подготовка" => "Thermal" + "\n" + "pre-treatment",
            "Блок Каталитической очистки" => "Catalytic" + "\n" + "cleaning unit",
            "1-я ступень" => "1st step",
            "2-я ступень" => "2nd step",
            "3-я ступень" => "3rd step",
            "Выход" => "Output",
            "Производство с применением золы" => "Ash production",
            "Удобрение" => "Fertiliser",
            "Отгрузка товарной углекислоты" => "Shipment of commercial" + "\n" + "carbon dioxide",
            "Удаление сажи и ТЦ" => "Soot and TC removal",
            "Удаление СО" => "CO removal",
            "Удаление остатков сажи и ТЦ" => "Removal of soot and TC residues",
            "Удаляется SO, NO2" => "SO, NO2 removal",
            "Сбор СО2" => "CO2 collection",
            "Очистной комплекс" => "Treatment plant",
            "Регенерация реагента" => "Reagent" + "\n" + "regeneration",
            "Дымовые газы" => "Flue gases",
            "Пульт управления" => "Control panel",
            "Горячий газ" => "Hot gas",
            "Компрессор КД" => "KD" + "\n" + "Compressor ",
            "Компрессор ВД" => "VD" + "\n" + "Compressor ",
            "Запуск" => "Start",
            "Стоп" => "Stop",
            "Пауза" => "Pause",
            "Назад" => "Back",
            "Частицы" => "Particles",
            "Напряжение" => "Tension",
            "Адсорбер 1" => "Adsorber 1",
            "Адсорбер 2" => "Adsorber 2",
            "конденсат H2O" => "H2O" + "\n" + "condensate",
            "Гипс" => "Gypsum",
            "Объем газа - V, м3/ч" => "Gas volume - V, m3/h",
            "Температура адцорбции - T, °C" => "Adsorption temperature - T, °C",
            "Температура дисорбции - T, °C" => "Disorption temperature - T, °C",
            "Длина гидравли. диаметра - D, м" => "Length of hydraulic diameter - D, m",
            "Скороть вентилятора - об/мин" => "Fan speed - rpm",
            _ => originalText
        };
    }

    public void SetLanguage(Language language)
    {
        currentLanguage = language;

        foreach (var textComponent in allTextComponents)
        {
            foreach (Transform child in textComponent.transform)
            {
                TextMeshProUGUI childText = child.GetComponent<TextMeshProUGUI>();
                if (childText != null)
                {
                    if (child.name == "KazakhText")
                        childText.enabled = (language == Language.Kazakh);
                    else if (child.name == "EnglishText")
                        childText.enabled = (language == Language.English);
                }
            }
            textComponent.enabled = (language == Language.Russian);
            //if (textComponent.transform.childCount > 0)
            //{
            //    textComponent.enabled = (language == Language.Russian);
            //}

        }
    }

    public void SetRussianLanguage()
    {
        SetLanguage(Language.Russian);
    }

    public void SetKazakhLanguage()
    {
        SetLanguage(Language.Kazakh);
    }

    public void SetEnglishLanguage()
    {
        SetLanguage(Language.English);
    }
}