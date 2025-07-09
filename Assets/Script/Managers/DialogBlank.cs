using System;
using System.Collections.Generic;

public enum Personality { Friendly, Greedy, Angry, Chill }

public static class DialogBank
{
    private static readonly System.Random prng = new System.Random();

    private static readonly Dictionary<Personality, string[]> AskItem = new()
    {
        [Personality.Friendly] = new[]
        {
            "Aku mau {0} {1}, harga satuannya berapa ya?",
            "Boleh dong {0} {1}, berapa sih satuannya?",
            "Aku suka {1}, {0} buah ya. Harganya satuan berapa?",
            "Mau beli {0} {1}, harga per buahnya berapa?",
            "Eh, {0} {1} dong. Satuan harganya berapa?",
            "Kalau aku ambil {0} {1}, harganya satuan berapa?",
            "Aku mau coba beli {0} {1}, berapa harga per {1}‑nya?",
            "Ambil {0} {1} boleh? Tapi satuannya berapa?",
            "Aku beli {0} {1} ya, satuannya berapa?",
            "Aku tertarik {0} {1}, kira‑kira berapa per satu?"
        },
        [Personality.Greedy] = new[]
        {
            "{0} {1}, tapi berapa satuannya? Jangan mahal ya!",
            "Gue mau {0} {1}, harga satuan berapa? Yang murah!",
            "{0} {1}, satuannya lo jual berapa?",
            "Satuan harga {1} berapa kalau gue beli {0}?",
            "Gue ambil {0} {1} kalau satuannya masuk akal.",
            "Mau {0} {1}, asal harga satuannya gak ngaco.",
            "Oke gue ambil {0} {1}, tapi satuannya jangan lebih dari modal ya.",
            "Harga satuan {1} berapa kalau gue beli {0}?",
            "{0} {1} lo jual berapa per satuannya?",
            "Beli {0} {1} deh, satuannya bisa dikorting?"
        },
        [Personality.Angry] = new[]
        {
            "Gue butuh {0} {1}, satuannya berapa cepet jawab!",
            "Langsung aja, {0} {1} gue mau. Satuan berapa!",
            "{1} {0} pcs. Harga satuan berapa!",
            "Nanya harga satuan itu susah ya? {0} {1}!",
            "{0} {1}, harganya per buahnya berapa sih!",
            "Buru‑buru nih. {0} {1}, satuannya?",
            "Ayo jawab! {0} {1}, harga satuannya berapa!",
            "Kalau lo jual {1}, satuannya berapa buat {0}?",
            "Satuan harga {1}! {0} buah!",
            "Gue beli {0} {1}, harga satuannya cepet kasih!"
        },
        [Personality.Chill] = new[]
        {
            "Yo bro, kalau {0} {1}, satuannya berapa?",
            "Gue mau {0} {1}, harga per satunya berapa ya?",
            "Pesen {0} {1} dong. Satuan harganya gimana?",
            "Kalau beli {0} {1}, satuan harga berapa?",
            "Slow aja, gue cuma nanya harga satuan buat {0} {1}.",
            "Bro, {0} {1} gue ambil. Harga satuannya?",
            "Penasaran nih, {0} {1}, berapa per itemnya?",
            "{0} {1} ya. Harganya per buah gimana?",
            "Gue ambil {0} {1} nih, satuannya kasih tau dong.",
            "Cuma mau tau, harga satuan {1} berapa buat {0}?"
        }
    };

    private static readonly Dictionary<Personality, string[]> BuyerHappy = new()
    {
        [Personality.Friendly] = new[]
        {
            "Sip! Makasih banyak!",
            "Deal, senang belanja di sini",
            "Mantap, cocok banget!",
            "Terima kasih, kamu baik sekali!",
            "Oke, bungkus!"
        },
        [Personality.Greedy] = new[]
        {
            "Heh, lumayan murah juga.",
            "Baiklah, gue ambil.",
            "Deal, tapi lain kali turunin lagi ya!",
            "Oke, dapet murah nih.",
            "Sip, harga pas kantong."
        },
        [Personality.Angry] = new[]
        {
            "Ya udah, gue beli.",
            "Hmm… akhirnya bener juga harganya.",
            "Deal. Next time jangan mahal.",
            "Bagus, selesai juga.",
            "Oke, sini barangnya!"
        },
        [Personality.Chill] = new[]
        {
            "Cool, gue ambil.",
            "Nice, cocok lah.",
            "Deal, makasih bro.",
            "Santai, jadi beli nih.",
            "Oke sip!"
        }
    };

    private static readonly Dictionary<Personality, string[]> BuyerReject = new()
    {
        [Personality.Friendly] = new[]
        {
            "Wah, sayang harganya belum cocok.",
            "Maaf ya, mungkin lain kali.",
            "Yah, gak jadi deh",
            "Hmm… gak bisa segitu.",
            "Belum rezeki, lain waktu ya."
        },
        [Personality.Greedy] = new[]
        {
            "Haha! Mahal banget, bye!",
            "Nope, gue tau harga pasar.",
            "Kemahalan. Next!",
            "Jangan becandain gue pake harga segitu.",
            "Skip, cari yang lebih murah."
        },
        [Personality.Angry] = new[]
        {
            "Apa?! Mahal! Gue cabut.",
            "Gila harganya! Bye.",
            "Nggak mau! Kelamaan!",
            "Males ah, mahal!",
            "Udah, gue pergi!"
        },
        [Personality.Chill] = new[]
        {
            "Hmm… nggak worth it.",
            "Ah, gak jadi deh.",
            "Skip dulu lah.",
            "Nanti aja kalo harga turun.",
            "Pass dulu, ya."
        }
    };

    public static string GetPartialAccept(Personality p) => p switch
    {
        Personality.Friendly => "Gak apa-apa, segitu juga cukup kok.",
        Personality.Greedy => "Yah... ambil dulu deh, tapi rugi nih.",
        Personality.Chill => "Oke aja, slow.",
        Personality.Angry => "Hmm... ya udah ambil.",
        _ => "Ambil aja lah."
    };

    public static string GetPartialReject(Personality p) => p switch
    {
        Personality.Friendly => "Wah, sayang banget. Aku butuhnya lebih.",
        Personality.Greedy => "Gak cukup! Cari tempat lain deh.",
        Personality.Chill => "Ah, ya udah. Nanti aja.",
        Personality.Angry => "Kurang?! Gak jadi beli!",
        _ => "Skip dulu."
    };

    public static string GetAskItem(Personality p, int qty, string item) => string.Format(RandomLine(AskItem[p]), qty, item);
    public static string GetHappy(Personality p) => RandomLine(BuyerHappy[p]);
    public static string GetReject(Personality p) => RandomLine(BuyerReject[p]);

    private static string RandomLine(IReadOnlyList<string> list) => list[prng.Next(list.Count)];
}