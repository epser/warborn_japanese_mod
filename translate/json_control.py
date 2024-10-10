#!/usr/bin/python

import sys
import json

# jsonファイルを読み込む
def load_json_file(file_path:str):
    with open(file_path, 'r', encoding='utf-8') as file:
        data = json.load(file)
    return data

# jsonファイルを書き込む
def write_json_file(file_path:str, data:dict):
    with open(file_path, 'w', encoding='utf-8') as file:
        json.dump(data, file, ensure_ascii=False, indent=4)

# 改行とタブをエスケープする
def escape_string(string:str):
    if string is None:
        return ""
    return string.replace("\r", "\\r").replace("\n", "\\n").replace("\t", "\\t")

# 改行とタブをアンエスケープする
def unescape_string(string:str):
    if string is None:
        return ""
    return string.replace("\\n", "\n").replace("\\t", "\t")

if __name__ == '__main__':

    options = {
        'export': '--export',
        'import': '--import',
    }

    # 引数が足りない場合はエラーを出力して終了
    if len(sys.argv) < 4:
        # コマンドヘルプ --export or --import
        print("Usage: python json_control.py --export <json_file_name> <tsv_file_name>")
        print("Usage: python json_control.py --import <json_file_name> <tsv_file_name>")
        sys.exit(1)

    # 第1引数がexport: 第2引数のJSONに二重格納されているJSONデータをTSV形式で出力する
    if sys.argv[1] == options['export']:
        json_file_name = sys.argv[2]
        tsv_file_name = sys.argv[3]

        json_all = load_json_file(json_file_name)
        inner_json = json.loads(json_all['m_Script'])

        # key-valueをTSV形式で変数に格納
        result = ""
        for key, value in inner_json.items():
            result += f'{key}\t{escape_string(value).strip()}' + "\n"

        # TSVファイルを出力
        with open(tsv_file_name, 'w', encoding='utf-8') as file:
            file.write(result)

    # 第1引数がimport: 第3引数のTSVファイルをJSONに変換して第2引数のJSONファイルに上書きする
    elif sys.argv[1] == options['import']:
        json_file_name = sys.argv[2]
        tsv_file_name = sys.argv[3]

        # TSVファイルを読み込む
        inner_json = {}
        with open(tsv_file_name, 'r', encoding='utf-8') as file:
            # TSVの1行目はヘッダーなのでスキップ
            file.readline()

            for line in file:
                # TSVをkey-valueの辞書に変換
                key, value = line.strip().split("\t")
                if value is not None:
                    inner_json[key] = unescape_string(value)

        # JSONファイルを読み込む
        json_all = load_json_file(json_file_name)
        json_all['m_Script'] = json.dumps(inner_json, ensure_ascii=False)

        # JSONファイルを上書き
        write_json_file(json_file_name, json_all)
