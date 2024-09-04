using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrieNode {
    public TrieNode[] children = new TrieNode[26];
    public bool isWord = false;
    public string word = ""; //i could check if word is empty but let's be normal about this
}

public class Trie
{
    public TrieNode root;
    public Trie() {
        root = new TrieNode();
    }

    public void AddWord(string word) {
        word = word.ToLower();

    }

    private void AddHelper(TrieNode curNode, string word, int index) {
        if (curNode == null) {
            return;
        } else if (index >= word.Length) {
            curNode.isWord = true;
            curNode.word = word;
            return;
        }

        int charId = word[index] - 'a';
        TrieNode nextNode = curNode.children[charId];
        if (nextNode == null) {
            nextNode = new TrieNode();
        }
        AddHelper(nextNode, word, index + 1);
    }

    public bool DoesWordExist(string word) {
        word = word.ToLower();
        TrieNode result = FindHelper(root, word, 0);
        return result != null && result.isWord;
    }

    public List<string> GetWordsWithPrefix(string prefix) {
        prefix= prefix.ToLower();
        TrieNode result = FindHelper(root, prefix, 0);
        if (result == null) {
            return null; //no word with prefix
        }

        List<string> words = new List<string>();
        GetAllWordsFromNode(result, words);
        return words;
    }

    private TrieNode FindHelper(TrieNode curNode, string word, int index) {
        if (curNode == null || index >= word.Length) {
            return curNode;
        } 

        int charId = word[index] - 'a';
        if (curNode.children[charId] != null) {
            TrieNode result = FindHelper(curNode.children[charId], word, index + 1);
            return result;
        }
        return curNode;
    }

    //given a node, gets all words that start from this node
    private void GetAllWordsFromNode(TrieNode curNode, List<string> words) {
        if (curNode == null) {
            return;
        } 

        if (curNode.isWord) {
            words.Add(curNode.word);
        }

        for (int i = 0; i < 26; i++) {
            if (curNode.children[i] != null) {
                GetAllWordsFromNode(curNode.children[i], words);
            }
        }
    }

}
